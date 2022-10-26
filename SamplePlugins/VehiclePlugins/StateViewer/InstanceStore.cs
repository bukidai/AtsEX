using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost;

namespace AtsEx.Samples.VehiclePlugins.StateViewer
{
    internal sealed class InstanceStore
    {
        public static InstanceStore Instance { get; private set; } = null;
        public static bool IsInitialized => !(Instance is null);

        public static void Initialize(BveHacker bveHacker)
        {
            Instance = new InstanceStore(bveHacker);
        }


        public BveHacker BveHacker { get; }

        private InstanceStore(BveHacker bveHacker)
        {
            BveHacker = bveHacker;
        }
    }
}
