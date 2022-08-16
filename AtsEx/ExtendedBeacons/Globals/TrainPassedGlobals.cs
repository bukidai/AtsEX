using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal class TrainPassedGlobals : ExtendedBeaconGlobalsBase<TrainPassedEventArgs>
    {
        public TrainPassedGlobals(PluginHost.BveHacker bveHacker, IReadOnlyDictionary<PluginType, PluginVariableCollection> pluginVariables,
            PluginHost.ExtendedBeacons.ExtendedBeaconBase<TrainPassedEventArgs> sender, TrainPassedEventArgs eventArgs)
            : base(bveHacker, pluginVariables, sender, eventArgs)
        {
        }

        internal override TrainPassedEventArgs GetEventArgsWithScriptVariables() => new TrainPassedEventArgs(e, Variables);
    }
}
