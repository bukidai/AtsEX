using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost;
using AtsEx.PluginHost.ExtendedBeacons;
using AtsEx.Scripting;

namespace AtsEx.ExtendedBeacons
{
    internal class InitializerBeacon : Beacon
    {
        private bool IsFirstTime = true;

        public InitializerBeacon(NativeImpl native, BveHacker bveHacker,
            string name, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, ObservingTargetTrain observingTargetTrain,
            IPluginScript<ExtendedBeaconGlobalsBase<PassedEventArgs>> script)
            : base(native, bveHacker, name, definedStructure, observingTargetTrack, observingTargetTrain, script)
        {
        }

        internal override void Tick(double currentLocation)
        {
            if (!IsFirstTime) return;

            NotifyPassed(Direction.Forward);
            IsFirstTime = false;
        }
    }
}
