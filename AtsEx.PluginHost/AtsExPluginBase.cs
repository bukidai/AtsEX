using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost
{
    /// <summary>
    /// AtsEX プラグインを表します。
    /// </summary>
    public abstract class AtsExPluginBase
    {
        public static bool HasInitialized { get; private set; } = false;

        public static IApp App { get; private set; }
        public static IBveHacker BveHacker { get; private set; }

        public static IVehicle Vehicle { get; private set; }
        public static IRoute Route { get; private set; }

        public static void Initialize(HostServiceCollection services)
        {
            HostServiceCollection pluginHost = services;

            App = pluginHost.App;
            BveHacker = pluginHost.BveHacker;

            Vehicle = pluginHost.Vehicle;
            Route = pluginHost.Route;

            HasInitialized = true;
        }

        public AtsExPluginBase()
        {
        }

        /// <summary>
        /// 毎フレーム呼び出されます。従来の ATS プラグインの Elapse(ATS_VEHICLESTATE vehicleState, int[] panel, int[] sound) に当たります。
        /// </summary>
        public abstract void Tick();
    }
}
