using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using UnembeddedResources;

using Automatic9045.AtsEx.Handles;
using Automatic9045.AtsEx.Input;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Input.Native;
using Automatic9045.AtsEx.PluginHost.Native;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx
{
    internal sealed class App : IApp
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<App>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ProductName { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ProductShortName { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        public static App Instance { get; private set; }

        static App()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private App(Process targetProcess, Assembly bveAssembly, Assembly atsExPluginHostAssembly)
        {
            Process = targetProcess;
            BveAssembly = bveAssembly;
            AtsExAssembly = Assembly.GetExecutingAssembly();
            AtsExPluginHostAssembly = atsExPluginHostAssembly;
            BveVersion = BveAssembly.GetName().Version;
        }

        public static void CreateInstance(Process targetProcess, Assembly bveAssembly, Assembly atsExPluginHostAssembly)
            => Instance = new App(targetProcess, bveAssembly, atsExPluginHostAssembly);

        public void SetScenario(VehicleSpec vehicleSpec)
        {
            VehicleSpec = vehicleSpec;

            BrakeHandle brake = new BrakeHandle(vehicleSpec.BrakeNotches, vehicleSpec.AtsNotch, vehicleSpec.B67Notch, false);
            PowerHandle power = new PowerHandle(VehicleSpec.PowerNotches);
            Reverser reverser = new Reverser();
            Handles = new HandleSet(brake, power, reverser);
        }

        public void InvokeStarted(BrakePosition defaultBrakePosition)
        {
            StartedEventArgs e = new StartedEventArgs(defaultBrakePosition);
            Started?.Invoke(e);
        }

        public string ProductName { get; } = Resources.Value.ProductName.Value;
        public string ProductShortName { get; } = Resources.Value.ProductShortName.Value;

        public Process Process { get; }
        public Assembly AtsExAssembly { get; }
        public Assembly AtsExPluginHostAssembly { get; }
        public Assembly BveAssembly { get; }
        public Version BveVersion { get; }

        private Dictionary<string, PluginBase> _VehiclePlugins = null;
        public Dictionary<string, PluginBase> VehiclePlugins
        {
            get => _VehiclePlugins is null ? throw new PropertyNotInitializedException(nameof(VehiclePlugins)) : _VehiclePlugins;
            set
            {
                _VehiclePlugins = value;
                ReadOnlyDictionary<string, PluginBase> readonlyDictionary = new ReadOnlyDictionary<string, PluginBase>(value);

                _Plugins[PluginType.VehiclePlugin] = readonlyDictionary;
                AllVehiclePluginLoaded?.Invoke(new AllPluginLoadedEventArgs(readonlyDictionary));
            }
        }

        private Dictionary<string, PluginBase> _MapPlugins = null;
        public Dictionary<string, PluginBase> MapPlugins
        {
            get => _MapPlugins is null ? throw new PropertyNotInitializedException(nameof(MapPlugins)) : _MapPlugins;
            set
            {
                _MapPlugins = value;
                ReadOnlyDictionary<string, PluginBase> readonlyDictionary = new ReadOnlyDictionary<string, PluginBase>(value);

                _Plugins[PluginType.MapPlugin] = readonlyDictionary;
                AllMapPluginLoaded?.Invoke(new AllPluginLoadedEventArgs(readonlyDictionary));
            }
        }

        private readonly Dictionary<PluginType, ReadOnlyDictionary<string, PluginBase>> _Plugins = new Dictionary<PluginType, ReadOnlyDictionary<string, PluginBase>>();
        public ReadOnlyDictionary<PluginType, ReadOnlyDictionary<string, PluginBase>> Plugins => new ReadOnlyDictionary<PluginType, ReadOnlyDictionary<string, PluginBase>>(_Plugins);

        private HandleSet _Handles = null;
        public HandleSet Handles
        {
            get => _Handles ?? throw new InvalidOperationException();
            set => _Handles = value;
        }

        public INativeKeySet NativeKeys { get; } = new NativeKeySet();

        private VehicleSpec _VehicleSpec = null;
        public VehicleSpec VehicleSpec
        {
            get => _VehicleSpec ?? throw new InvalidOperationException();
            private set => _VehicleSpec = value;
        }

        public VehicleState VehicleState { get; set; } = null;

        public event AllPluginLoadedEventHandler AllVehiclePluginLoaded;
        public event AllPluginLoadedEventHandler AllMapPluginLoaded;
        public event StartedEventHandler Started;
    }
}
