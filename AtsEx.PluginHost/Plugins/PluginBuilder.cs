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
        internal protected IBveHacker BveHacker { get; private set; } = null;

        internal protected PluginBuilder(IApp app)
        {
            App = app;
        }

        protected PluginBuilder(PluginBuilder pluginBuilder)
        {
            App = pluginBuilder.App;
            BveHacker = pluginBuilder.BveHacker;
        }

        internal protected PluginBuilder UseAtsExExtensions(IBveHacker bveHacker)
        {
            BveHacker = bveHacker;
            return this;
        }
    }
}
