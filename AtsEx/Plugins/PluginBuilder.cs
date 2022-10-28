using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Plugins
{
    internal sealed class PluginBuilder : PluginHost.Plugins.PluginBuilder
    {
        public PluginLoader CreatedBy { get; }

        protected override event AllPluginsLoadedEventHandler AllPluginsLoaded;

        public PluginBuilder(INative native, PluginHost.BveHacker bveHacker, string identifier, PluginLoader createdBy) : base(native, bveHacker, identifier)
        {
            CreatedBy = createdBy;
            CreatedBy.AllPluginsLoaded += e => AllPluginsLoaded?.Invoke(e);
        }

        public AllPluginsLoadedEventHandler GetAllPluginsLoaded() => AllPluginsLoaded;
    }
}
