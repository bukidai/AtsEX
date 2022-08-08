using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal class TrainPassedGlobals : ExtendedBeaconGlobalsBase<TrainPassedEventArgs>
    {
        public TrainPassedGlobals(PluginHost.BveHacker bveHacker, PluginHost.ExtendedBeacons.ExtendedBeaconBase<TrainPassedEventArgs> sender, TrainPassedEventArgs eventArgs) : base(bveHacker, sender, eventArgs)
        {
        }

        internal override TrainPassedEventArgs GetEventArgsWithScriptVariables() => new TrainPassedEventArgs(e, Variables);
    }
}
