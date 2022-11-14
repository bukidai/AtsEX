using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.Handles;
using AtsEx.Input;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Native;

namespace AtsEx
{
    internal class NativeImpl : INative
    {
        public NativeImpl(VehicleSpec vehicleSpec)
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

        public PluginHost.Handles.HandleSet Handles { get; }

        public INativeKeySet NativeKeys { get; } = new NativeKeySet();

        public VehicleSpec VehicleSpec { get; }

        public VehicleState VehicleState { get; set; } = null;

        public event StartedEventHandler Started;
    }
}
