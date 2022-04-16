using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx
{
    internal class CoreHackService
    {
        protected Process TargetProcess { get; }
        protected ServiceCollection Services { get; }

        public CoreHackService(Process targetProcess, ServiceCollection services)
        {
            TargetProcess = targetProcess;
            Services = services;
        }
    }
}
