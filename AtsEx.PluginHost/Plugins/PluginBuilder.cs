using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Plugins
{
    public class PluginBuilder
    {
        internal protected IApp App { get; }
        internal protected BveHacker BveHacker { get; }
        internal protected string Identifier { get; }

        public PluginBuilder(IApp app, BveHacker bveHacker, string identifier)
        {
            App = app;
            BveHacker = bveHacker;
            Identifier = identifier;
        }

        protected PluginBuilder(PluginBuilder pluginBuilder)
        {
            App = pluginBuilder.App;
            BveHacker = pluginBuilder.BveHacker;
            Identifier = pluginBuilder.Identifier;
        }
    }
}
