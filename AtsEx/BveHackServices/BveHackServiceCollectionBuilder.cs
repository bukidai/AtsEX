using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveHackServices;

namespace Automatic9045.AtsEx
{

    internal static class BveHackServiceCollectionBuilder
    {
        public static ServiceCollection Build()
        {
            ServiceCollection services = new ServiceCollection();

            services.Register<ITimeHacker>(() => new TimeHacker(services));
            services.Register<ILocationHacker>(() => new LocationHacker(services));
            services.Register<ISpeedHacker>(() => new SpeedHacker(services));

            return services;
        }
    }
}
