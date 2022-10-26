using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Plugins
{
    public class PluginBuilder
    {
        internal protected IScenarioService ScenarioService { get; }
        internal protected BveHacker BveHacker { get; }
        internal protected string Identifier { get; }

        public PluginBuilder(IScenarioService scenarioService, BveHacker bveHacker, string identifier)
        {
            ScenarioService = scenarioService;
            BveHacker = bveHacker;
            Identifier = identifier;
        }

        protected PluginBuilder(PluginBuilder pluginBuilder)
        {
            ScenarioService = pluginBuilder.ScenarioService;
            BveHacker = pluginBuilder.BveHacker;
            Identifier = pluginBuilder.Identifier;
        }
    }
}
