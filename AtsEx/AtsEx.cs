using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;

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

            BveTypeCollectionProvider.CreateInstance(TargetAssembly, ExecutingAssembly, PluginHostAssembly);

            App.CreateInstance(TargetAssembly, ExecutingAssembly, PluginHostAssembly);
            BveHacker.CreateInstance(TargetProcess);

            BveHackServices = BveHackServiceCollectionBuilder.Build();
            Vehicle = new Vehicle(BveHackServices);
            Route = new Route(BveHackServices);

            VersionFormProvider = new VersionFormProvider();

            AssemblyResolver assemblyResolver = new AssemblyResolver(TargetAppDomain);
            AtsExPluginLoader pluginLoader = new AtsExPluginLoader(Vehicle, Route, assemblyResolver);
            try
            {
                string vehiclePluginListPath = Path.Combine(Path.GetDirectoryName(ExecutingAssembly.Location), "atsex.pilist.txt");
                VehiclePlugins = pluginLoader.LoadFromList(PluginType.VehiclePlugin, vehiclePluginListPath).ToList();
                App.Instance.VehiclePlugins = VehiclePlugins;
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

            MapPlugins = new List<AtsExPluginInfo>();
            App.Instance.MapPlugins = MapPlugins;
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

        public void Elapse()
        {
            App.Instance.InvokeElapse();
        }
    }
}
