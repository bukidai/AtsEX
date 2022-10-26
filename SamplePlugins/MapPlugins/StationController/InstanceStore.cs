using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost;

namespace AtsEx.Samples.MapPlugins.StationController
{
    internal sealed class InstanceStore
    {
        public static InstanceStore Instance { get; private set; } = null;
        public static bool IsInitialized => !(Instance is null);

        public static void Initialize(IScenarioService scenarioService, BveHacker bveHacker)
        {
            Instance = new InstanceStore(scenarioService, bveHacker);
        }


        public IScenarioService ScenarioService { get; }
        public BveHacker BveHacker { get; }

        private InstanceStore(IScenarioService scenarioService, BveHacker bveHacker)
        {
            ScenarioService = scenarioService;
            BveHacker = bveHacker;
        }
    }
}
