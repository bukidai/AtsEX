using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal class CoreHackService
    {
        protected Process TargetProcess { get; }
        protected Assembly TargetAssembly { get; }
        protected ServiceCollection Services { get; }

        public CoreHackService(Process targetProcess, Assembly targetAssembly, ServiceCollection services)
        {
            TargetProcess = targetProcess;
            TargetAssembly = targetAssembly;
            Services = services;
        }
    }
}
