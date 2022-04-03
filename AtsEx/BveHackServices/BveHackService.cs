using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx
{

    internal class BveHackService
    {
        public ServiceCollection Services { get; protected set; }

        public BveHackService(ServiceCollection services)
        {
            Services = services;
        }
    }
}
