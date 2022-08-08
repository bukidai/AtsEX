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

        public PluginBuilder(IApp app, BveHacker bveHacker)
        {
            App = app;
            BveHacker = bveHacker;
        }

        protected PluginBuilder(PluginBuilder pluginBuilder)
        {
            App = pluginBuilder.App;
            BveHacker = pluginBuilder.BveHacker;
        }
    }
}
