using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.Input;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Helpers;
using Automatic9045.AtsEx.PluginHost.Input.Native;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx
{
    public sealed class AtsEx : IDisposable
    {
        private readonly Process TargetProcess;
        private readonly AppDomain TargetAppDomain;
        private readonly Assembly TargetAssembly;
        private readonly Assembly CallerAssembly;
        private readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        private readonly Assembly PluginHostAssembly;

        private readonly App App;
        private readonly BveHacker BveHacker;

        private readonly ContextMenuHacker ContextMenuHacker;

        private readonly VersionFormProvider VersionFormProvider;

        private readonly List<AtsExPluginInfo> VehiclePlugins;
        private readonly List<AtsExPluginInfo> MapPlugins;

        public AtsEx(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly, Assembly callerAssembly)
        {
            string pluginHostAssemblyPath = Path.Combine(Path.GetDirectoryName(ExecutingAssembly.Location), "AtsEx.PluginHost.dll");
            PluginHostAssembly = Assembly.LoadFrom(pluginHostAssemblyPath);

            TargetProcess = targetProcess;
            TargetAppDomain = targetAppDomain;
            TargetAssembly = targetAssembly;
            CallerAssembly = callerAssembly;

            Stopwatch sw = new Stopwatch();

            sw.Restart();
            ResourceLocalizer resources = ResourceLocalizer.FromResXOfType<AtsEx>("Core");
            Debug.WriteLine($"ResourceLocalizer: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            Version bveVersion = TargetAssembly.GetName().Version;
            Version profileVersion = BveTypeCollectionProvider.CreateInstance(TargetAssembly, ExecutingAssembly, PluginHostAssembly, true);
            Debug.WriteLine($"BveTypeCollectionProvider: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            App = new App(TargetAssembly, CallerAssembly, ExecutingAssembly, PluginHostAssembly);
            Debug.WriteLine($"App: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            BveHacker = new BveHacker(TargetProcess);
            Debug.WriteLine($"BveHacker: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            ClassWrapperInitializer classWrapperInitializer = new ClassWrapperInitializer(App, BveHacker);
            classWrapperInitializer.InitializeAll();
            Debug.WriteLine($"ClassWrapper: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            HelperInitializer helperInitializer = new HelperInitializer(App, BveHacker);
            helperInitializer.InitializeAll();
            Debug.WriteLine($"ClassWrapper: {sw.ElapsedMilliseconds}ms");

            //DXDynamicTextureHost = new DXDynamicTextureHost();

            string versionWarningText = string.Format(resources.GetString("BveVersionNotSupported").Value, bveVersion, profileVersion, App.ProductShortName);
            if (profileVersion != bveVersion)
            {
                LoadErrorManager.Throw(versionWarningText);
            }

            ContextMenuHacker = new ContextMenuHacker();
            ContextMenuHacker.AddSeparator();

            VersionFormProvider = new VersionFormProvider(App, BveHacker);

            PluginLoader pluginLoader = new PluginLoader(App, BveHacker);
            try
            {
                {
                    sw.Restart();
                    string vehiclePluginListPath = Path.Combine(Path.GetDirectoryName(CallerAssembly.Location), "AtsEx.VehiclePluginList.txt");
                    VehiclePlugins = pluginLoader.LoadFromList(PluginType.VehiclePlugin, vehiclePluginListPath).ToList();
                    Debug.WriteLine($"VehiclePlugins: {sw.ElapsedMilliseconds}ms");
                }

                if (profileVersion != bveVersion && VehiclePlugins.All(plugin => !plugin.PluginInstance.UseAtsExExtensions))
                {
                    LoadError removeTargetError = LoadErrorManager.Errors.FirstOrDefault(error => error.Text == versionWarningText);
                    if (!(removeTargetError is null))
                    {
                        LoadErrorManager.Errors.Remove(removeTargetError);
                    }
                }

                {
                    sw.Restart();
                    MapLoader mapLoader = new MapLoader(BveHacker, pluginLoader);
                    mapLoader.Load();
                    MapPlugins = mapLoader.LoadedPlugins;
                    Debug.WriteLine($"MapPlugins: {sw.ElapsedMilliseconds}ms");

                    IEnumerable<LoadError> removeTargetErrors = LoadErrorManager.Errors.Where(error =>
                    {
                        if (error.Text.Contains("[[NOMPI]]")) return true;

                        bool isMapPluginUsingError = mapLoader.RemoveErrorIncludePositions.
                            Any(pos => Path.GetFileName(pos.Key) == error.SenderFileName && pos.Value.Contains((error.LineIndex, error.CharIndex)));
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
                MessageBox.Show(ex.ToString(), string.Format(resources.GetString("UnhandledExceptionCaption").Value, App.ProductShortName));
            }
            finally
            {
                if (VehiclePlugins is null) VehiclePlugins = new List<AtsExPluginInfo>();
                if (MapPlugins is null) MapPlugins = new List<AtsExPluginInfo>();

                App.VehiclePlugins = VehiclePlugins;
                App.MapPlugins = MapPlugins;
            }

            VersionFormProvider.Intialize(Enumerable.Concat(VehiclePlugins, MapPlugins));
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
            ContextMenuHacker.Dispose();
            BveTypeCollectionProvider.Instance.Dispose();
        }

        [WillRefactor]
        public void SetVehicleSpec(VehicleSpec vehicleSpec)
        {
            BveHacker.VehicleSpec = vehicleSpec;
        }

        public void Started(BrakePosition defaultBrakePosition)
        {
            App.InvokeStarted(defaultBrakePosition);
        }

        [WillRefactor]
        public void Tick(VehicleState vehicleState)
        {
            BveHacker.VehicleState = vehicleState;

            VehiclePlugins.ForEach(plugin => plugin.PluginInstance.Tick());
            MapPlugins.ForEach(plugin => plugin.PluginInstance.Tick());
        }

        public void KeyDown(NativeAtsKeyName key)
        {
            (App.NativeKeys.AtsKeys[key] as NativeAtsKey).Press();
        }

        public void KeyUp(NativeAtsKeyName key)
        {
            (App.NativeKeys.AtsKeys[key] as NativeAtsKey).Release();
        }
    }
}
