using AtsEx.PluginHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.ExtendedBeacons;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.ExtendedBeacons
{
    internal class PassedGlobals : ExtendedBeaconGlobalsBase<PassedEventArgs>
    {
        public PassedGlobals(INative native, PluginHost.BveHacker bveHacker,
            PluginHost.ExtendedBeacons.ExtendedBeaconBase<PassedEventArgs> sender, PassedEventArgs eventArgs)
            : base(native, bveHacker, sender, eventArgs)
        {
        }

        internal override PassedEventArgs GetEventArgsWithScriptVariables() => new PassedEventArgs(e, Variables);
    }
}
