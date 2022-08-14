using Automatic9045.AtsEx.PluginHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal class PassedGlobals : ExtendedBeaconGlobalsBase<PassedEventArgs>
    {
        public PassedGlobals(PluginHost.BveHacker bveHacker, IReadOnlyDictionary<PluginType, PluginVariableCollection> pluginVariables,
            PluginHost.ExtendedBeacons.ExtendedBeaconBase<PassedEventArgs> sender, PassedEventArgs eventArgs)
            : base(bveHacker, pluginVariables, sender, eventArgs)
        {
        }

        internal override PassedEventArgs GetEventArgsWithScriptVariables() => new PassedEventArgs(e, Variables);
    }
}
