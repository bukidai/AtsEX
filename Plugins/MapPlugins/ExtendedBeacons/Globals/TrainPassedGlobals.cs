using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost;

namespace AtsEx.MapPlugins.ExtendedBeacons
{
    internal class TrainPassedGlobals : ExtendedBeaconGlobalsBase<TrainPassedEventArgs>
    {
        public TrainPassedGlobals(INative native, BveHacker bveHacker, ExtendedBeaconBase<TrainPassedEventArgs> sender, TrainPassedEventArgs eventArgs)
            : base(native, bveHacker, sender, eventArgs)
        {
        }

        internal override TrainPassedEventArgs GetEventArgsWithScriptVariables() => new TrainPassedEventArgs(e, Variables);
    }
}
