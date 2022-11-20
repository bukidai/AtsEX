using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.Plugins.Extensions;

namespace AtsEx.PluginHost.Plugins
{
    public abstract class PluginBuilder
    {
        internal protected INative Native { get; }
        internal protected BveHacker BveHacker { get; }
        internal protected IExtensionSet Extensions { get; }
        internal protected string Identifier { get; }

        internal protected abstract event AllExtensionsLoadedEventHandler AllExtensionsLoaded;
        internal protected abstract event AllPluginsLoadedEventHandler AllPluginsLoaded;

        public PluginBuilder(INative native, BveHacker bveHacker, IExtensionSet extensions, string identifier)
        {
            Native = native;
            BveHacker = bveHacker;
            Extensions = extensions;
            Identifier = identifier;
        }

        protected PluginBuilder(PluginBuilder pluginBuilder)
        {
            Native = pluginBuilder.Native;
            BveHacker = pluginBuilder.BveHacker;
            Extensions = pluginBuilder.Extensions;
            Identifier = pluginBuilder.Identifier;
        }
    }
}
