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

namespace Automatic9045.AtsEx
{
    internal sealed class AtsEx : IDisposable
    {
        private Process TargetProcess { get; }
        private AppDomain TargetAppDomain { get; }
        private Assembly TargetAssembly { get; }
        private Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();

        private App App { get; }
        private BveHacker BveHacker { get; }

        private ServiceCollection BveHackServices { get; }
        private Vehicle Vehicle { get; }
        private Route Route { get; }

        private VersionFormProvider VersionFormProvider { get; }

        private List<AtsExPluginInfo> VehiclePlugins { get; }
        private List<AtsExPluginInfo> MapPlugins { get; }

        [UnderConstruction]
        public AtsEx(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly)
        {
            TargetProcess = targetProcess;
            TargetAppDomain = targetAppDomain;
            TargetAssembly = targetAssembly;

            App = new App(ExecutingAssembly, TargetAssembly);
            BveHacker = new BveHacker(App, TargetProcess, TargetAssembly);

            BveHackServices = BveHackServiceCollectionBuilder.Build(BveHacker);
            Vehicle = new Vehicle(BveHacker, BveHackServices);
            Route = new Route(BveHacker, BveHackServices);

            VersionFormProvider = new VersionFormProvider(App, BveHacker);

            AssemblyResolver assemblyResolver = new AssemblyResolver(TargetAppDomain);
            AtsExPluginLoader pluginLoader = new AtsExPluginLoader(App, BveHacker, Vehicle, Route, assemblyResolver);
            try
            {
                string vehiclePluginListPath = Path.Combine(Path.GetDirectoryName(ExecutingAssembly.Location), "atsex.pilist.txt");
                VehiclePlugins = pluginLoader.LoadFromList(PluginType.VehiclePlugin, vehiclePluginListPath).ToList();
                App.VehiclePlugins = VehiclePlugins;
            }
            catch (BveFileLoadException ex)
            {
                MessageBox.Show(ex.ToString(), $"エラー - {App.ProductShortName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), $"ハンドルされていない例外 - {App.ProductShortName}");
            }

            MapPlugins = new List<AtsExPluginInfo>();
            App.MapPlugins = MapPlugins;
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
            BveHacker.Dispose();
        }

        public void Started(BrakePosition defaultBrakePosition)
        {
            VersionFormProvider.Intialize(Enumerable.Concat(VehiclePlugins, MapPlugins));

            App.InvokeStarted(defaultBrakePosition);
        }

        public void Elapse()
        {
            App.InvokeElapse();
        }
    }
}
