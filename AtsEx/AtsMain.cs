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
    /// <summary>メインの機能をここに実装する。</summary>
    internal static class AtsMain
    {
        public static VehicleSpec VehicleSpec { get; set; }

        /// <summary>Is the Door Closed TF</summary>
        public static bool IsDoorClosed { get; set; } = false;

        /// <summary>Current State of Handles</summary>
        public static Handles Handle = default;

        public static VehicleState VehicleState { get; set; } = default;

        /// <summary>Current Key State</summary>
        public static bool[] IsKeyDown { get; set; } = new bool[16];

        public static AtsEx AtsEx { get; private set; }

        public static void Load(Assembly callerAssembly, AtsExActivator activator)
        {
            AtsEx = new AtsEx(activator.TargetProcess, activator.TargetAppDomain, activator.TargetAssembly, callerAssembly);
        }

        public static void Dispose()
        {
            AtsEx?.Dispose();
        }

        public static void SetVehicleSpec(VehicleSpec vehicleSpec)
        {
            PluginHost.VehicleSpec exVehicleSpec = new PluginHost.VehicleSpec(vehicleSpec.BrakeNotches, vehicleSpec.PowerNotches, vehicleSpec.AtsNotch, vehicleSpec.B67Notch, vehicleSpec.Cars);

            AtsEx?.SetVehicleSpec(exVehicleSpec);
        }

        public static void Initialize(int defaultBrakePosition)
        {
            AtsEx?.Started((BrakePosition)defaultBrakePosition);
        }

        public static void Elapse(VehicleState vehicleState, int[] panel, int[] sound)
        {
            PluginHost.VehicleState exVehicleState = new PluginHost.VehicleState(
                vehicleState.Location, vehicleState.Speed,
                vehicleState.BcPressure, vehicleState.MrPressure, vehicleState.ErPressure, vehicleState.BpPressure, vehicleState.SapPressure, vehicleState.Current);

            AtsEx?.Tick(exVehicleState);
        }

        public static void SetPower(int notch)
        {

        }

        public static void SetBrake(int notch)
        {

        }

        public static void SetReverser(int position)
        {

        }
        public static void KeyDown(int atsKeyCode)
        {

        }

        public static void KeyUp(int atsKeyCode)
        {

        }

        public static void DoorOpen()
        {

        }
        public static void DoorClose()
        {

        }
        public static void HornBlow(HornType hornType)
        {

        }
        public static void SetSignal(int signal)
        {

        }
        public static void SetBeaconData(BeaconData beaconData)
        {

        }
    }
}
