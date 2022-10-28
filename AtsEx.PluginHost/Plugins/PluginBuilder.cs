using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Plugins
{
    public abstract class PluginBuilder
    {
        internal protected INative Native { get; }
        internal protected BveHacker BveHacker { get; }
        internal protected string Identifier { get; }

        internal protected abstract event AllPluginsLoadedEventHandler AllPluginsLoaded;

        public PluginBuilder(INative native, BveHacker bveHacker, string identifier)
        {
            Native = native;
            BveHacker = bveHacker;
            Identifier = identifier;
        }

        protected PluginBuilder(PluginBuilder pluginBuilder)
        {
            Native = pluginBuilder.Native;
            BveHacker = pluginBuilder.BveHacker;
            Identifier = pluginBuilder.Identifier;
        }
    }
}
