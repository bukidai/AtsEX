using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Helpers;

namespace Automatic9045.AtsEx
{
    public sealed class AtsEx : IDisposable
    {
        private Process TargetProcess { get; }
        private AppDomain TargetAppDomain { get; }
        private Assembly TargetAssembly { get; }
        private Assembly CallerAssembly { get; }
        private Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();
        private Assembly PluginHostAssembly { get; }

        private VersionFormProvider VersionFormProvider { get; }

        private List<AtsExPluginInfo> VehiclePlugins { get; }
        private List<AtsExPluginInfo> MapPlugins { get; }

        public AtsEx(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly, Assembly callerAssembly)
        {
            string pluginHostAssemblyPath = Path.Combine(Path.GetDirectoryName(ExecutingAssembly.Location), "atsex.pihost.dll");
            PluginHostAssembly = Assembly.LoadFrom(pluginHostAssemblyPath);

            TargetProcess = targetProcess;
            TargetAppDomain = targetAppDomain;
            TargetAssembly = targetAssembly;
            CallerAssembly = callerAssembly;

            Version bveVersion = TargetAssembly.GetName().Version;
            Version profileVersion = BveTypeCollectionProvider.CreateInstance(TargetAssembly, ExecutingAssembly, PluginHostAssembly, true);

            App.CreateInstance(TargetAssembly, CallerAssembly, ExecutingAssembly, PluginHostAssembly);
            BveHacker.CreateInstance(TargetProcess);

            InstanceStore.Initialize(App.Instance, BveHacker.Instance);

            if (profileVersion != bveVersion)
            {
                LoadErrorManager.Throw($"BVE バージョン {bveVersion} には対応していません。" +
                    $"{profileVersion} 向けのプロファイルで代用しますが、{App.Instance.ProductShortName} による拡張機能は正常に動作しない可能性があります。");
            }

            VersionFormProvider = new VersionFormProvider();

            PluginLoader pluginLoader = new PluginLoader();
            try
            {
                {
                    string vehiclePluginListPath = Path.Combine(Path.GetDirectoryName(CallerAssembly.Location), "atsex.pilist.txt");
                    VehiclePlugins = pluginLoader.LoadFromList(PluginType.VehiclePlugin, vehiclePluginListPath).ToList();
                }

                {
                    MapLoader mapLoader = new MapLoader(pluginLoader);
                    mapLoader.Load();
                    MapPlugins = mapLoader.LoadedPlugins;

                    IEnumerable<LoadError> removeTargetErrors = LoadErrorManager.Errors.Where(error =>
                    {
                        if (error.Text.Contains("[[NOMPI]]")) return true;

                        bool isMapPluginUsingError = mapLoader.RemoveErrorIncludePositions.Any(pos =>
                        {
                            if (Path.GetFileName(pos.Key) != error.SenderFileName) return false;
                            return pos.Value.Contains((error.LineIndex, error.CharIndex));
                        });
                        return isMapPluginUsingError;
                    });
                    foreach (LoadError error in removeTargetErrors)
                    {
                        LoadErrorManager.Errors.Remove(error);
                    }
                }
            }
            catch (BveFileLoadException ex)
            {
                LoadErrorManager.Throw(ex.Message, ex.SenderFileName, ex.LineIndex, ex.CharIndex);
            }
            catch (Exception ex)
            {
                LoadErrorManager.Throw(ex.Message);
                MessageBox.Show(ex.ToString(), $"ハンドルされていない例外 - {App.Instance.ProductShortName}");
            }
            finally
            {
                if (VehiclePlugins is null) VehiclePlugins = new List<AtsExPluginInfo>();
                if (MapPlugins is null) MapPlugins = new List<AtsExPluginInfo>();

                App.Instance.VehiclePlugins = VehiclePlugins;
                App.Instance.MapPlugins = MapPlugins;
            }
        }

        public void Dispose()
        {
            VehiclePlugins.ForEach(plugin =>
            {
                if (plugin.PluginInstance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            });

            MapPlugins.ForEach(plugin =>
            {
                if (plugin.PluginInstance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            });

            VersionFormProvider.Dispose();
            InstanceStore.Dispose();
            BveTypeCollectionProvider.Instance.Dispose();
        }

        [WillRefactor]
        public void SetVehicleSpec(VehicleSpec vehicleSpec)
        {
            BveHacker.Instance.VehicleSpec = vehicleSpec;
        }

        public void Started(BrakePosition defaultBrakePosition)
        {
            VersionFormProvider.Intialize(Enumerable.Concat(VehiclePlugins, MapPlugins));

            App.Instance.InvokeStarted(defaultBrakePosition);
        }

        [WillRefactor]
        public void Tick(VehicleState vehicleState)
        {
            BveHacker.Instance.VehicleState = vehicleState;

            VehiclePlugins.ForEach(plugin => plugin.PluginInstance.Tick());
            MapPlugins.ForEach(plugin => plugin.PluginInstance.Tick());
        }
    }
}
