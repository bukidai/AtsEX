using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx
{

    internal class BveHackService
    {
        public BveHacker BveHacker { get; protected set; }
        public ServiceCollection Services { get; protected set; }

        public BveHackService(BveHacker bveHacker, ServiceCollection services)
        {
            BveHacker = bveHacker;
            Services = services;
        }
    }
}
