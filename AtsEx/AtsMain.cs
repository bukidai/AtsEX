using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Input.Native;

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

        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static AtsEx AtsEx;

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

            AtsEx?.Tick(Stopwatch.IsRunning ? Stopwatch.Elapsed : TimeSpan.Zero, exVehicleState);

            Stopwatch.Restart();
        }

        public static void KeyDown(int atsKeyCode)
        {
            AtsEx.KeyDown((NativeAtsKeyName)atsKeyCode);
        }

        public static void KeyUp(int atsKeyCode)
        {
            AtsEx.KeyUp((NativeAtsKeyName)atsKeyCode);
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
