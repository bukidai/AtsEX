using Automatic9045.AtsEx.PluginHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal class PassedGlobals : ExtendedBeaconGlobalsBase<PassedEventArgs>
    {
        public PassedGlobals(IBveHacker bveHacker, PluginHost.ExtendedBeacons.ExtendedBeaconBase<PassedEventArgs> sender, PassedEventArgs eventArgs) : base(bveHacker, sender, eventArgs)
        {
        }
    }
}
