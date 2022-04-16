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

namespace Automatic9045.AtsEx
{
    internal sealed class AtsEx : IDisposable
    {
        private Process TargetProcess { get; }
        private AppDomain TargetAppDomain { get; }
        private Assembly TargetAssembly { get; }
        private Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();
        private Assembly PluginHostAssembly { get; }

        private ServiceCollection BveHackServices { get; }
        private Vehicle Vehicle { get; }
        private Route Route { get; }

        private VersionFormProvider VersionFormProvider { get; }

        private List<AtsExPluginInfo> VehiclePlugins { get; }
        private List<AtsExPluginInfo> MapPlugins { get; }

        public AtsEx(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly)
        {
            string pluginHostAssemblyPath = Path.Combine(Path.GetDirectoryName(ExecutingAssembly.Location), "atsex.pihost.dll");
            PluginHostAssembly = Assembly.LoadFrom(pluginHostAssemblyPath);

            TargetProcess = targetProcess;
            TargetAppDomain = targetAppDomain;
            TargetAssembly = targetAssembly;

            {
                Version bveVersion = TargetAssembly.GetName().Version;
                Version profileVersion = BveTypeCollectionProvider.CreateInstance(TargetAssembly, ExecutingAssembly, PluginHostAssembly, true);
                if (profileVersion != bveVersion)
                {
                    BveHacker.Instance.ThrowError($"BVE バージョン {bveVersion} には対応していません。" +
                        $"{profileVersion} 向けのプロファイルで代用しますが、{App.Instance.ProductShortName} による拡張機能は正常に動作しない可能性があります。");
                }
            }

            App.CreateInstance(TargetAssembly, ExecutingAssembly, PluginHostAssembly);
            BveHacker.CreateInstance(TargetProcess);

            BveHackServices = BveHackServiceCollectionBuilder.Build();
            Vehicle = new Vehicle(BveHackServices);
            Route = new Route(BveHackServices);

            VersionFormProvider = new VersionFormProvider();

            AssemblyResolver assemblyResolver = new AssemblyResolver(TargetAppDomain);
            PluginLoader pluginLoader = new PluginLoader(Vehicle, Route, assemblyResolver);
            try
            {
                {
                    string vehiclePluginListPath = Path.Combine(Path.GetDirectoryName(ExecutingAssembly.Location), "atsex.pilist.txt");
                    VehiclePlugins = pluginLoader.LoadFromList(PluginType.VehiclePlugin, vehiclePluginListPath).ToList();
                }

                {
                    MapLoader mapLoader = new MapLoader(pluginLoader);
                    mapLoader.Load();
                    MapPlugins = mapLoader.LoadedPlugins;

                    // TODO: NOMPI、MPIUSINGのエラーの除去
                }
            }
            catch (BveFileLoadException ex)
            {
                BveHacker.Instance.ThrowError(ex.Message, ex.SenderFileName, ex.LineIndex, ex.CharIndex);
            }
            catch (Exception ex)
            {
                BveHacker.Instance.ThrowError(ex.Message);
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
            BveHackServices.Dispose();
            BveHacker.Instance.Dispose();
            BveTypeCollectionProvider.Instance.Dispose();
        }

        public void Started(BrakePosition defaultBrakePosition)
        {
            VersionFormProvider.Intialize(Enumerable.Concat(VehiclePlugins, MapPlugins));

            App.Instance.InvokeStarted(defaultBrakePosition);
        }

        public void Tick()
        {
            VehiclePlugins.ForEach(plugin => plugin.PluginInstance.Tick());
            MapPlugins.ForEach(plugin => plugin.PluginInstance.Tick());
        }
    }
}
