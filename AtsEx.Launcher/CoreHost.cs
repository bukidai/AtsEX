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
        internal CoreHost(Assembly callerAssembly, AtsExActivator activator)
        {
            Assembly launcherAssembly = Assembly.GetExecutingAssembly();
            CallerInfo callerInfo = new CallerInfo(activator.TargetProcess, activator.TargetAppDomain, activator.TargetAssembly, callerAssembly, launcherAssembly);

            Export.Load(callerInfo);
        }

        public void Dispose() => Export.Dispose();
        public void SetVehicleSpec(VehicleSpec vehicleSpec) => Export.SetVehicleSpec(vehicleSpec.Convert());
        public void Initialize(int defaultBrakePosition) => Export.Initialize((DefaultBrakePosition)defaultBrakePosition);
        public AtsHandles Elapse(VehicleState vehicleState, int[] panel, int[] sound)
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
