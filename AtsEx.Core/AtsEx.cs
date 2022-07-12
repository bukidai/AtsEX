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
using Automatic9045.AtsEx.Plugins;
using Automatic9045.AtsEx.Plugins.Scripting.CSharp;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.BveTypes;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Helpers;
using Automatic9045.AtsEx.PluginHost.Input.Native;
using Automatic9045.AtsEx.PluginHost.Plugins;
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

        private readonly BveHacker BveHacker;

        private readonly ContextMenuHacker ContextMenuHacker;

        private readonly VersionFormProvider VersionFormProvider;

        private readonly List<PluginBase> VehiclePlugins;
        private readonly List<PluginBase> MapPlugins;

        public AtsEx(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly, Assembly callerAssembly)
        {
            string pluginHostAssemblyPath = Path.Combine(Path.GetDirectoryName(ExecutingAssembly.Location), "AtsEx.PluginHost.dll");
            PluginHostAssembly = Assembly.LoadFrom(pluginHostAssemblyPath);

            TargetProcess = targetProcess;
            TargetAppDomain = targetAppDomain;
            TargetAssembly = targetAssembly;
            CallerAssembly = callerAssembly;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            {
                AssemblyName assemblyName = new AssemblyName(e.Name);
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyName.Name + ".dll");
                return File.Exists(path) ? Assembly.LoadFrom(path) : null;
            };

            Stopwatch sw = new Stopwatch();

            sw.Restart();
            ResourceLocalizer resources = ResourceLocalizer.FromResXOfType<AtsEx>("Core");
            Debug.WriteLine($"ResourceLocalizer: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            Version bveVersion = TargetAssembly.GetName().Version;
            Version profileVersion = BveTypeSet.CreateInstance(TargetAssembly, ExecutingAssembly, PluginHostAssembly, true);
            Debug.WriteLine($"BveTypeCollectionProvider: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            App.CreateInstance(TargetAssembly, CallerAssembly, ExecutingAssembly, PluginHostAssembly);
            Debug.WriteLine($"App: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            BveHacker = new BveHacker(TargetProcess);
            Debug.WriteLine($"BveHacker: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            ClassWrapperInitializer classWrapperInitializer = new ClassWrapperInitializer(App.Instance, BveHacker);
            classWrapperInitializer.InitializeAll();
            Debug.WriteLine($"ClassWrapper: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            HelperInitializer helperInitializer = new HelperInitializer(App.Instance, BveHacker);
            helperInitializer.InitializeAll();
            Debug.WriteLine($"ClassWrapper: {sw.ElapsedMilliseconds}ms");

            //DXDynamicTextureHost = new DXDynamicTextureHost();

            string versionWarningText = string.Format(resources.GetString("BveVersionNotSupported").Value, bveVersion, profileVersion, App.Instance.ProductShortName);
            if (profileVersion != bveVersion)
            {
                LoadErrorManager.Throw(versionWarningText);
            }

            ContextMenuHacker = new ContextMenuHacker();
            ContextMenuHacker.AddSeparator();

            VersionFormProvider = new VersionFormProvider(BveHacker);

            PluginLoader pluginLoader = new PluginLoader(BveHacker);
            try
            {
                {
                    sw.Restart();
                    string vehiclePluginUsingPath = Path.Combine(Path.GetDirectoryName(CallerAssembly.Location), Path.GetFileNameWithoutExtension(CallerAssembly.Location) + ".VehiclePluginUsing.xml");
                    PluginUsing vehiclePluginUsing = PluginUsing.Load(PluginType.VehiclePlugin, vehiclePluginUsingPath);
                    VehiclePlugins = pluginLoader.LoadFromPluginUsing(vehiclePluginUsing).ToList();
                    Debug.WriteLine($"VehiclePlugins: {sw.ElapsedMilliseconds}ms");
                }

                if (profileVersion != bveVersion && VehiclePlugins.All(plugin => !plugin.UseAtsExExtensions))
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
            catch (CompilationException ex)
            {
                ex.ThrowAsLoadError();
            }
            catch (BveFileLoadException ex)
            {
                LoadErrorManager.Throw(ex.Message, ex.SenderFileName, ex.LineIndex, ex.CharIndex);
            }
            catch (Exception ex)
            {
                LoadErrorManager.Throw(ex.Message);
                MessageBox.Show(ex.ToString(), string.Format(resources.GetString("UnhandledExceptionCaption").Value, App.Instance.ProductShortName));
            }
            finally
            {
                if (VehiclePlugins is null) VehiclePlugins = new List<PluginBase>();
                if (MapPlugins is null) MapPlugins = new List<PluginBase>();

                App.Instance.VehiclePlugins = VehiclePlugins;
                App.Instance.MapPlugins = MapPlugins;
            }

            VersionFormProvider.Intialize(Enumerable.Concat(VehiclePlugins, MapPlugins));
        }

        public void Dispose()
        {
            VehiclePlugins.ForEach(plugin =>
            {
                if (plugin is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            });

            MapPlugins.ForEach(plugin =>
            {
                if (plugin is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            });

            VersionFormProvider.Dispose();
            ContextMenuHacker.Dispose();
            BveTypeSet.Instance.Dispose();
        }

        public void SetVehicleSpec(VehicleSpec vehicleSpec)
        {
            App.Instance.VehicleSpec = vehicleSpec;
        }

        public void Started(BrakePosition defaultBrakePosition)
        {
            App.Instance.InvokeStarted(defaultBrakePosition);
        }

        [WillRefactor]
        public void Tick(TimeSpan elapsed, VehicleState vehicleState)
        {
            App.Instance.VehicleState = vehicleState;

            VehiclePlugins.ForEach(plugin => plugin.Tick(elapsed));
            MapPlugins.ForEach(plugin => plugin.Tick(elapsed));
        }

        public void KeyDown(NativeAtsKeyName key)
        {
            (App.Instance.NativeKeys.AtsKeys[key] as NativeAtsKey).NotifyPressed();
        }

        public void KeyUp(NativeAtsKeyName key)
        {
            (App.Instance.NativeKeys.AtsKeys[key] as NativeAtsKey).NotifyReleased();
        }
    }
}
