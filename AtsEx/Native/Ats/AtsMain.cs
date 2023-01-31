using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BveTypes.ClassWrappers;

using AtsEx.Handles;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Native;

namespace AtsEx.Native
{
    /// <summary>メインの機能をここに実装する。</summary>
    internal static class AtsMain
    {
        public static VehicleSpec VehicleSpec { get; set; }

        /// <summary>Is the Door Closed TF</summary>
        public static bool IsDoorClosed { get; set; } = false;

        private static CallerInfo CallerInfo;

        private static readonly Stopwatch Stopwatch = new Stopwatch();

        private static AtsEx.AsAtsPlugin AtsEx;
        private static ScenarioService.AsAtsPlugin AtsExScenarioService;

        public static void Load(CallerInfo callerInfo)
        {
            CallerInfo = callerInfo;

            Version callerVersion = CallerInfo.AtsExCallerAssembly.GetName().Version;
            if (callerVersion < new Version(0, 16))
            {
                string errorMessage = $"読み込まれた AtsEX Caller (バージョン {callerVersion}) は現在の AtsEX ではサポートされていません。\nbeta0.16 (バージョン 0.16) 以降の Ats Caller をご利用下さい。";
                MessageBox.Show(errorMessage, "AtsEX Caller バージョンエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new NotSupportedException(errorMessage.Replace("\n", ""));
            }

            AtsEx = new AtsEx.AsAtsPlugin(CallerInfo);
        }

        public static void Dispose()
        {
            AtsExScenarioService?.Dispose();
            AtsEx?.Dispose();
        }

        public static void SetVehicleSpec(VehicleSpec vehicleSpec)
        {
            PluginHost.Native.VehicleSpec exVehicleSpec = new PluginHost.Native.VehicleSpec(
                vehicleSpec.BrakeNotches, vehicleSpec.PowerNotches, vehicleSpec.AtsNotch, vehicleSpec.B67Notch, vehicleSpec.Cars);

            AtsExScenarioService = new ScenarioService.AsAtsPlugin(AtsEx, CallerInfo.AtsExCallerAssembly, exVehicleSpec);
        }

        public static void Initialize(DefaultBrakePosition defaultBrakePosition)
        {
            AtsExScenarioService?.Started((BrakePosition)defaultBrakePosition);
        }

        public static AtsHandles Elapse(VehicleState vehicleState, IntPtr panel, IntPtr sound)
        {
            AtsIoArray panelArray = new AtsIoArray(panel);
            AtsIoArray soundArray = new AtsIoArray(sound);

            PluginHost.Native.VehicleState exVehicleState = new PluginHost.Native.VehicleState(
                vehicleState.Location, vehicleState.Speed, TimeSpan.FromMilliseconds(vehicleState.Time),
                vehicleState.BcPressure, vehicleState.MrPressure, vehicleState.ErPressure, vehicleState.BpPressure, vehicleState.SapPressure, vehicleState.Current);

            TimeSpan elapsed = Stopwatch.IsRunning ? Stopwatch.Elapsed : TimeSpan.Zero;
            AtsEx.Tick(elapsed);
            HandlePositionSet handlePositionSet = AtsExScenarioService?.Tick(elapsed, exVehicleState, panelArray, soundArray);

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

        public static void KeyDown(ATSKeys atsKeyCode)
        {
            AtsExScenarioService?.KeyDown((NativeAtsKeyName)atsKeyCode);
        }

        public static void KeyUp(ATSKeys atsKeyCode)
        {
            AtsExScenarioService?.KeyUp((NativeAtsKeyName)atsKeyCode);
        }

        public static void DoorOpen()
        {
            AtsExScenarioService?.DoorOpened(new DoorEventArgs());
        }
        public static void DoorClose()
        {
            AtsExScenarioService?.DoorClosed(new DoorEventArgs());
        }
        public static void HornBlow(HornType hornType)
        {

        }
        public static void SetSignal(int signal)
        {

        }
        public static void SetBeaconData(BeaconData beaconData)
        {
            BeaconPassedEventArgs args = new BeaconPassedEventArgs(beaconData.Num, beaconData.Sig, beaconData.Z, beaconData.Data);
            AtsExScenarioService?.BeaconPassed(args);
        }
    }
}
