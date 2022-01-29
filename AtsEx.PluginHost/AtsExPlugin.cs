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
    public class AtsExPlugin
    {
        protected IApp App { get; }
        protected IBveHacker BveHacker { get; }

        protected IVehicle Vehicle { get; }
        protected IRoute Route { get; }

        public AtsExPlugin(IHostServiceCollection services)
        {
            HostServiceCollection pluginHost = services as HostServiceCollection;

            App = pluginHost.App;
            BveHacker = pluginHost.BveHacker;

            Vehicle = pluginHost.Vehicle;
            Route = pluginHost.Route;
        }
    }
}
