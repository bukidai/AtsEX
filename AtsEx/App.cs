using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.Handles;
using Automatic9045.AtsEx.Input;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Input.Native;
using Automatic9045.AtsEx.PluginHost.Plugins;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx
{
    internal sealed class App : IApp
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<App>("Core");

        public static App Instance { get; private set; }

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

        public string ProductName { get; } = Resources.GetString("ProductName").Value;
        public string ProductShortName { get; } = Resources.GetString("ProductShortName").Value;

        public Process Process { get; }
        public Assembly AtsExAssembly { get; }
        public Assembly AtsExPluginHostAssembly { get; }
        public Assembly BveAssembly { get; }
        public Version BveVersion { get; }

        private SortedList<string, PluginBase> _VehiclePlugins = null;
        public SortedList<string, PluginBase> VehiclePlugins
        {
            get => _VehiclePlugins is null ? throw new PropertyNotInitializedException(nameof(VehiclePlugins)) : _VehiclePlugins;
            set
            {
                _VehiclePlugins = value;
                _Plugins[PluginType.VehiclePlugin] = new ReadOnlyDictionary<string, PluginBase>(value);
                AllVehiclePluginLoaded?.Invoke(new AllPluginLoadedEventArgs(_VehiclePlugins));
            }
        }

        private SortedList<string, PluginBase> _MapPlugins = null;
        public SortedList<string, PluginBase> MapPlugins
        {
            get => _MapPlugins is null ? throw new PropertyNotInitializedException(nameof(MapPlugins)) : _MapPlugins;
            set
            {
                _MapPlugins = value;
                _Plugins[PluginType.MapPlugin] = new ReadOnlyDictionary<string, PluginBase>(value);
                AllMapPluginLoaded?.Invoke(new AllPluginLoadedEventArgs(_MapPlugins));
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
