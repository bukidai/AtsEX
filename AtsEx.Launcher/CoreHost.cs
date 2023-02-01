using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Launcher.Hosting;

using AtsEx.Native;

namespace AtsEx.Launcher
{
    public class CoreHost : IDisposable
    {
        private const int LauncherVersion = 2;
        internal CoreHost(Assembly callerAssembly, TargetBveFinder bveFinder)
        {
            Assembly launcherAssembly = Assembly.GetExecutingAssembly();
            CallerInfo callerInfo = new CallerInfo(bveFinder.TargetProcess, bveFinder.TargetAppDomain, bveFinder.TargetAssembly, callerAssembly, launcherAssembly);

            Assembly atsExAssembly = typeof(Export).Assembly;
            LauncherCompatibilityVersionAttribute compatibilityVersionAttribute = atsExAssembly.GetCustomAttribute<LauncherCompatibilityVersionAttribute>();
            if (compatibilityVersionAttribute.Version != LauncherVersion)
            {
                Version launcherAssemblyVersion = launcherAssembly.GetName().Version;
                throw new NotSupportedException($"読み込まれた AtsEX Launcher (バージョン {launcherAssemblyVersion}) は現在の AtsEX ではサポートされていません。");
            }

            Export.Load(callerInfo);
        }

        public void Dispose() => Export.Dispose();
        public void SetVehicleSpec(VehicleSpec vehicleSpec) => Export.SetVehicleSpec(vehicleSpec.Convert());
        public void Initialize(int defaultBrakePosition) => Export.Initialize((DefaultBrakePosition)defaultBrakePosition);
        public AtsHandles Elapse(VehicleState vehicleState, IntPtr panel, IntPtr sound)
        {
            Native.AtsHandles handles = Export.Elapse(vehicleState.Convert(), panel, sound);
            return new AtsHandles()
            {
                Brake = handles.Brake,
                Power = handles.Power,
                Reverser = handles.Reverser,
                ConstantSpeed = handles.ConstantSpeed,
            };
        }
        public void SetPower(int notch) => Export.SetPower(notch);
        public void SetBrake(int notch) => Export.SetBrake(notch);
        public void SetReverser(int position) => Export.SetReverser(position);
        public void KeyDown(int atsKeyCode) => Export.KeyDown((ATSKeys)atsKeyCode);
        public void KeyUp(int atsKeyCode) => Export.KeyUp((ATSKeys)atsKeyCode);
        public void HornBlow(int hornType) => Export.HornBlow((HornType)hornType);
        public void DoorOpen() => Export.DoorOpen();
        public void DoorClose() => Export.DoorClose();
        public void SetSignal(int signal) => Export.SetSignal(signal);
        public void SetBeaconData(BeaconData beaconData) => Export.SetBeaconData(beaconData.Convert());
    }
}
