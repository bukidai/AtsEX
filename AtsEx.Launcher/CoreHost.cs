using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Launcher.Hosting;

using AtsEx.Native;
using AtsEx.Native.Ats;

namespace AtsEx.Launcher
{
    public class CoreHost : IDisposable
    {
        private readonly CallerInfo CallerInfo;

        internal CoreHost(Assembly callerAssembly, TargetBveFinder bveFinder)
        {
            LauncherVersionChecker.Check();

            Assembly launcherAssembly = Assembly.GetExecutingAssembly();
            CallerInfo = new CallerInfo(bveFinder.TargetProcess, bveFinder.TargetAppDomain, bveFinder.TargetAssembly, callerAssembly, launcherAssembly);

            AtsMain.Load(CallerInfo);
        }

        public void Dispose() => AtsMain.Dispose();
        public void SetVehicleSpec(VehicleSpec vehicleSpec) => AtsMain.SetVehicleSpec(vehicleSpec.Convert());
        public void Initialize(int defaultBrakePosition) => AtsMain.Initialize((DefaultBrakePosition)defaultBrakePosition);
        public AtsHandles Elapse(VehicleState vehicleState, IntPtr panel, IntPtr sound)
        {
            Native.AtsHandles handles = AtsMain.Elapse(vehicleState.Convert(), panel, sound);
            return new AtsHandles()
            {
                Brake = handles.Brake,
                Power = handles.Power,
                Reverser = handles.Reverser,
                ConstantSpeed = handles.ConstantSpeed,
            };
        }
        [Obsolete("使用不可", true)]
        public AtsHandles Elapse(VehicleState vehicleState, int[] panel, int[] sound)
        {
            Version callerAssemblyVersion = CallerInfo.AtsExCallerAssembly.GetName().Version;
            throw new NotSupportedException($"読み込まれた AtsEX Caller (バージョン {callerAssemblyVersion}) は現在の AtsEX ではサポートされていません。");
        }
        public void SetPower(int notch) => AtsMain.SetPower(notch);
        public void SetBrake(int notch) => AtsMain.SetBrake(notch);
        public void SetReverser(int position) => AtsMain.SetReverser(position);
        public void KeyDown(int atsKeyCode) => AtsMain.KeyDown((ATSKeys)atsKeyCode);
        public void KeyUp(int atsKeyCode) => AtsMain.KeyUp((ATSKeys)atsKeyCode);
        public void HornBlow(int hornType) => AtsMain.HornBlow((HornType)hornType);
        public void DoorOpen() => AtsMain.DoorOpen();
        public void DoorClose() => AtsMain.DoorClose();
        public void SetSignal(int signal) => AtsMain.SetSignal(signal);
        public void SetBeaconData(BeaconData beaconData) => AtsMain.SetBeaconData(beaconData.Convert());
    }
}
