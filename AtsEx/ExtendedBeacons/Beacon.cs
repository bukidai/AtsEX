using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.Plugins.Scripting;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal class Beacon : ExtendedBeaconBase<PassedEventArgs>
    {
        private double OldLocation = 0d;

        public Beacon(BveHacker bveHacker, IReadOnlyDictionary<PluginType, PluginVariableCollection> pluginVariables,
            string name, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, ObservingTargetTrain observingTargetTrain,
            IPluginScript<ExtendedBeaconGlobalsBase<PassedEventArgs>> script)
            : base(bveHacker, pluginVariables, name, definedStructure, observingTargetTrack, observingTargetTrain, script)
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
            PassedGlobals globals = new PassedGlobals(BveHacker, PluginVariables, this, eventArgs);
            Script.RunAsync(globals).Wait();
            base.NotifyPassed(globals.GetEventArgsWithScriptVariables());
        }
    }
}
