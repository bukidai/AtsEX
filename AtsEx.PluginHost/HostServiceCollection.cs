using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    public class HostServiceCollection : IHostServiceCollection
    {
        internal IApp App { get; }
        internal IBveHacker BveHacker { get; }

        internal IVehicle Vehicle { get; }
        internal IRoute Route { get; }

        public HostServiceCollection(IApp app, IBveHacker bveHacker, IVehicle vehicle, IRoute route)
        {
            App = app;
            BveHacker = bveHacker;

            Vehicle = vehicle;
            Route = route;
        }
    }
}
