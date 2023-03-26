using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BveTypes;
using BveTypes.ClassWrappers;
using UnembeddedResources;

using AtsEx.Handles;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Native;

namespace AtsEx.Native.Ats
{
    /// <summary>メインの機能をここに実装する。</summary>
    public static class AtsMain
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType(typeof(AtsMain), "Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> BveVersionNotSupported { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static AtsMain()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private static CallerInfo CallerInfo;

        private static readonly Stopwatch Stopwatch = new Stopwatch();

        private static string VersionWarningText;

        private static AtsEx.AsAtsPlugin AtsEx;
        private static ScenarioService.AsAtsPlugin ScenarioService;

        private static int Power;
        private static int Brake;
        private static int Reverser;

        public static void Load(CallerInfo callerInfo)
        {
            if (App.IsInitialized) return;

            CallerInfo = callerInfo;

            Version callerVersion = CallerInfo.AtsExCallerAssembly.GetName().Version;
            if (callerVersion < new Version(0, 16))
            {
                string errorMessage = $"読み込まれた AtsEX Caller (バージョン {callerVersion}) は現在の AtsEX ではサポートされていません。\nbeta0.16 (バージョン 0.16) 以降の Ats Caller をご利用下さい。";
                MessageBox.Show(errorMessage, "AtsEX Caller バージョンエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new NotSupportedException(errorMessage.Replace("\n", ""));
            }

            AppInitializer.Initialize(CallerInfo, LaunchMode.Ats);

            BveTypeSetLoader bveTypesLoader = new BveTypeSetLoader();
            BveTypeSetLoader.ProfileForDifferentVersionBveLoadedEventArgs args = null;
            bveTypesLoader.ProfileForDifferentVersionBveLoaded += (sender, e) => args = e;

            BveTypeSet bveTypes = bveTypesLoader.Load();

            AtsEx = new AtsEx.AsAtsPlugin(bveTypes);

            if (!(args is null))
            {
                VersionWarningText = string.Format(Resources.Value.BveVersionNotSupported.Value, args.BveVersion, args.ProfileVersion, App.Instance.ProductShortName);
                AtsEx.BveHacker.LoadErrorManager.Throw(VersionWarningText);
            }
        }

        public static void Dispose()
        {
            ScenarioService?.Dispose();
            AtsEx?.Dispose();
        }

        public static void SetVehicleSpec(VehicleSpec vehicleSpec)
        {
            if (App.Instance.LaunchMode != LaunchMode.Ats) return;

            PluginHost.Native.VehicleSpec exVehicleSpec = new PluginHost.Native.VehicleSpec(
                vehicleSpec.BrakeNotches, vehicleSpec.PowerNotches, vehicleSpec.AtsNotch, vehicleSpec.B67Notch, vehicleSpec.Cars);

            ScenarioService = new ScenarioService.AsAtsPlugin(AtsEx, CallerInfo.AtsExCallerAssembly, exVehicleSpec, VersionWarningText);
        }

        public static void Initialize(DefaultBrakePosition defaultBrakePosition)
        {
            if (App.Instance.LaunchMode != LaunchMode.Ats) return;

            ScenarioService?.Started((BrakePosition)defaultBrakePosition);
        }

        public static AtsHandles Elapse(VehicleState vehicleState, IntPtr panel, IntPtr sound)
        {
            if (App.Instance.LaunchMode != LaunchMode.Ats)
            {
                return new AtsHandles()
                {
                    Brake = Brake,
                    Power = Power,
                    Reverser = Reverser,
                    ConstantSpeed = 0,
                };
            }

            AtsIoArray panelArray = new AtsIoArray(panel);
            AtsIoArray soundArray = new AtsIoArray(sound);

            PluginHost.Native.VehicleState exVehicleState = new PluginHost.Native.VehicleState(
                vehicleState.Location, vehicleState.Speed, TimeSpan.FromMilliseconds(vehicleState.Time),
                vehicleState.BcPressure, vehicleState.MrPressure, vehicleState.ErPressure, vehicleState.BpPressure, vehicleState.SapPressure, vehicleState.Current);

            TimeSpan elapsed = Stopwatch.IsRunning ? Stopwatch.Elapsed : TimeSpan.Zero;
            AtsEx.Tick(elapsed);
            HandlePositionSet handlePositionSet = ScenarioService?.Tick(elapsed, exVehicleState, panelArray, soundArray);

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
            Power = notch;

            if (App.Instance.LaunchMode != LaunchMode.Ats) return;

            ScenarioService?.SetPower(notch);
        }

        public static void SetBrake(int notch)
        {
            Brake = notch;

            ScenarioService?.SetBrake(notch);
        }

        public static void SetReverser(int position)
        {
            Reverser = position;

            ScenarioService?.SetReverser((ReverserPosition)position);
        }

        public static void KeyDown(ATSKeys atsKeyCode)
        {
            if (App.Instance.LaunchMode != LaunchMode.Ats) return;

            ScenarioService?.KeyDown((NativeAtsKeyName)atsKeyCode);
        }

        public static void KeyUp(ATSKeys atsKeyCode)
        {
            if (App.Instance.LaunchMode != LaunchMode.Ats) return;

            ScenarioService?.KeyUp((NativeAtsKeyName)atsKeyCode);
        }

        public static void DoorOpen()
        {
            if (App.Instance.LaunchMode != LaunchMode.Ats) return;

            ScenarioService?.DoorOpened(new DoorEventArgs());
        }
        public static void DoorClose()
        {
            if (App.Instance.LaunchMode != LaunchMode.Ats) return;

            ScenarioService?.DoorClosed(new DoorEventArgs());
        }
        public static void HornBlow(HornType hornType)
        {

        }
        public static void SetSignal(int signal)
        {

        }
        public static void SetBeaconData(BeaconData beaconData)
        {
            if (App.Instance.LaunchMode != LaunchMode.Ats) return;

            BeaconPassedEventArgs args = new BeaconPassedEventArgs(beaconData.Num, beaconData.Sig, beaconData.Z, beaconData.Data);
            ScenarioService?.BeaconPassed(args);
        }
    }
}
