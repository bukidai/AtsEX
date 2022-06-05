using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    public class AtsExPluginBuilder
    {
        internal IApp App { get; }
        internal IBveHacker BveHacker { get; private set; } = null;

        public AtsExPluginBuilder(IApp app)
        {
            App = app;
        }

        public AtsExPluginBuilder UseAtsExExtensions(IBveHacker bveHacker)
        {
            BveHacker = bveHacker;
            return this;
        }
    }
}
