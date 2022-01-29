using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.CoreHackServices;

namespace Automatic9045.AtsEx
{

    internal static class CoreHackServiceCollectionBuilder
    {
        public static ServiceCollection Build(Process targetProcess, Assembly targetAssembly)
        {
            ServiceCollection services = new ServiceCollection();

            services.Register<IMainFormHacker>(() => new MainFormHacker(targetProcess, targetAssembly, services));
            services.Register<IContextMenuHacker>(() => new ContextMenuHacker(targetProcess, targetAssembly, services));
            services.Register<ISubFormHacker>(() => new SubFormHacker(targetProcess, targetAssembly, services));
            services.Register<IScenarioHacker>(() => new ScenarioHacker(targetProcess, targetAssembly, services));

            return services;
        }
    }
}
