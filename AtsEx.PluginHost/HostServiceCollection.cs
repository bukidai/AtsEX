using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    public class HostServiceCollection
    {
        internal IApp App { get; }
        internal IBveHacker BveHacker { get; }

        public HostServiceCollection(IApp app, IBveHacker bveHacker)
        {
            App = app;
            BveHacker = bveHacker;
        }
    }
}
