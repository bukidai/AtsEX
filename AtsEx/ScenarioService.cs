using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Handles;
using AtsEx.Input;
using AtsEx.PluginHost;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Native;
using AtsEx.PluginHost.Plugins;

namespace AtsEx
{
    internal class ScenarioService : IScenarioService
    {
        public ScenarioService(VehicleSpec vehicleSpec)
        {
            VehicleSpec = vehicleSpec;

            BrakeHandle brake = new BrakeHandle(vehicleSpec.BrakeNotches, vehicleSpec.AtsNotch, vehicleSpec.B67Notch, false);
            PowerHandle power = new PowerHandle(VehicleSpec.PowerNotches);
            Reverser reverser = new Reverser();
            Handles = new PluginHost.Handles.HandleSet(brake, power, reverser);
        }

        public void InvokeStarted(BrakePosition defaultBrakePosition)
        {
            StartedEventArgs e = new StartedEventArgs(defaultBrakePosition);
            Started?.Invoke(e);
        }

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

        public PluginHost.Handles.HandleSet Handles { get; }

        public INativeKeySet NativeKeys { get; } = new NativeKeySet();

        public VehicleSpec VehicleSpec { get; }

        public VehicleState VehicleState { get; set; } = null;

        public event AllPluginLoadedEventHandler AllVehiclePluginLoaded;
        public event AllPluginLoadedEventHandler AllMapPluginLoaded;
        public event StartedEventHandler Started;
    }
}
