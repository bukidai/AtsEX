using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.Ats;
using Automatic9045.AtsEx.BveHackServices;
using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx
{
    internal class Route : BveHackService, IRoute
    {
        public Route(ServiceCollection services) : base(services)
        {
        }

        public int TimeMilliseconds
        {
            get => AtsMain.VehicleState.Time;
        }

        public DateTime Time
        {
            get => Services.GetService<ITimeHacker>().Time;
            set => Services.GetService<ITimeHacker>().Time = value;
        }
    }
}
