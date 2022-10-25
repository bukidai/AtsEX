using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Plugins.Scripting;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.ExtendedBeacons;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.ExtendedBeacons
{
    internal class InitializerBeacon : Beacon
    {
        private bool IsFirstTime = true;

        public InitializerBeacon(ScenarioService scenarioService, BveHacker bveHacker,
            string name, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, ObservingTargetTrain observingTargetTrain,
            IPluginScript<ExtendedBeaconGlobalsBase<PassedEventArgs>> script)
            : base(scenarioService, bveHacker, name, definedStructure, observingTargetTrack, observingTargetTrain, script)
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
