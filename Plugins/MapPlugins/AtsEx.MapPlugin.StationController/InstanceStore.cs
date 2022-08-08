using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.MapPlugins.StationController
{
    internal sealed class InstanceStore
    {
        public static InstanceStore Instance { get; private set; } = null;
        public static bool IsInitialized => !(Instance is null);

        public static void Initialize(IApp app, BveHacker bveHacker)
        {
            Instance = new InstanceStore(app, bveHacker);
        }


        public IApp App { get; }
        public BveHacker BveHacker { get; }

        private InstanceStore(IApp app, BveHacker bveHacker)
        {
            App = app;
            BveHacker = bveHacker;
        }
    }
}
