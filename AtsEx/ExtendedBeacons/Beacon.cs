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
    internal class Beacon : ExtendedBeaconBase<PassedEventArgs>
    {
        private double OldLocation = 0d;

        public Beacon(ScenarioService scenarioService, BveHacker bveHacker,
            string name, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, ObservingTargetTrain observingTargetTrain,
            IPluginScript<ExtendedBeaconGlobalsBase<PassedEventArgs>> script)
            : base(scenarioService, bveHacker, name, definedStructure, observingTargetTrack, observingTargetTrain, script)
        {
        }

        internal virtual void Tick(double currentLocation)
        {
            if (OldLocation < Location && Location <= currentLocation)
            {
                NotifyPassed(Direction.Forward);
            }
            else if (Location < OldLocation && currentLocation <= Location)
            {
                NotifyPassed(Direction.Backward);
            }

            OldLocation = currentLocation;
        }

        protected void NotifyPassed(Direction direction)
        {
            PassedEventArgs eventArgs = new PassedEventArgs(direction);
            PassedGlobals globals = new PassedGlobals(ScenarioService, BveHacker, this, eventArgs);
            Script.Run(globals);
            base.NotifyPassed(globals.GetEventArgsWithScriptVariables());
        }
    }
}
