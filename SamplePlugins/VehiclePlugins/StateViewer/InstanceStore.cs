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

        public static void Initialize(INative native, BveHacker bveHacker)
        {
            Instance = new InstanceStore(native, bveHacker);
        }


        public INative Native { get; }
        public BveHacker BveHacker { get; }

        private InstanceStore(INative native, BveHacker bveHacker)
        {
            Native = native;
            BveHacker = bveHacker;
        }
    }
}
