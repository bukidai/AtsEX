using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.Hosting;

using Automatic9045.AtsEx.Handles;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Input.Native;

namespace Automatic9045.AtsEx.Native
{
    /// <summary>メインの機能をここに実装する。</summary>
    internal static class AtsMain
    {
        public static VehicleSpec VehicleSpec { get; set; }

        /// <summary>Is the Door Closed TF</summary>
        public static bool IsDoorClosed { get; set; } = false;

        private static Assembly CallerAssembly;
        private static AtsExActivator Activator;

        private static readonly Stopwatch Stopwatch = new Stopwatch();

        private static AtsEx.AsAtsPlugin AtsEx;
        private static AtsExScenarioService.AsAtsPlugin AtsExScenarioService;

        public static void LoadAsInputDevice(Activator activator)
        {

        }

        public static void Load(Assembly callerAssembly, AtsExActivator activator)
        {
            CallerAssembly = callerAssembly;
            Activator = activator;
        }

        public static void Dispose()
        {
            AtsExScenarioService?.Dispose();
            AtsEx?.Dispose();
        }

        public static void SetVehicleSpec(VehicleSpec vehicleSpec)
        {
            PluginHost.VehicleSpec exVehicleSpec = new PluginHost.VehicleSpec(vehicleSpec.BrakeNotches, vehicleSpec.PowerNotches, vehicleSpec.AtsNotch, vehicleSpec.B67Notch, vehicleSpec.Cars);

            AtsEx = new AtsEx.AsAtsPlugin(Activator.TargetProcess, Activator.TargetAppDomain, Activator.TargetAssembly);
            AtsExScenarioService = new AtsExScenarioService.AsAtsPlugin(AtsEx, CallerAssembly, exVehicleSpec);
        }

        public static void Initialize(int defaultBrakePosition)
        {
            AtsExScenarioService?.Started((BrakePosition)defaultBrakePosition);
        }

        public static AtsHandles Elapse(VehicleState vehicleState, int[] panel, int[] sound)
        {
            PluginHost.VehicleState exVehicleState = new PluginHost.VehicleState(
                vehicleState.Location, vehicleState.Speed,
                vehicleState.BcPressure, vehicleState.MrPressure, vehicleState.ErPressure, vehicleState.BpPressure, vehicleState.SapPressure, vehicleState.Current);

            HandlePositionSet handlePositionSet = AtsExScenarioService?.Tick(Stopwatch.IsRunning ? Stopwatch.Elapsed : TimeSpan.Zero, exVehicleState);

            Stopwatch.Restart();

            return new AtsHandles()
            {
                Brake = handlePositionSet.Brake,
                Power = handlePositionSet.Power,
                Reverser = (int)handlePositionSet.ReverserPosition,
                ConstantSpeed = (int)handlePositionSet.ConstantSpeed,
            };
        }

        public static void SetPower(int notch)
        {
            AtsExScenarioService?.SetPower(notch);
        }

        public static void SetBrake(int notch)
        {
            AtsExScenarioService?.SetBrake(notch);
        }

        public static void SetReverser(int position)
        {
            AtsExScenarioService?.SetReverser((ReverserPosition)position);
        }

        public static void KeyDown(int atsKeyCode)
        {
            AtsExScenarioService?.KeyDown((NativeAtsKeyName)atsKeyCode);
        }

        public static void KeyUp(int atsKeyCode)
        {
            AtsExScenarioService?.KeyUp((NativeAtsKeyName)atsKeyCode);
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
