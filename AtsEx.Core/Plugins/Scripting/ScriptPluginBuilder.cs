using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.Plugins.Scripting
{
    internal sealed class ScriptPluginBuilder : PluginBuilder
    {
        public string Location { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }

        public IPluginScript<Globals> ConstructorScript { get; set; }
        public IPluginScript<Globals> DisposeScript { get; set; }
        public IPluginScript<ScenarioCreatedGlobals> OnScenarioCreatedScript { get; set; }
        public IPluginScript<StartedGlobals> OnStartedScript { get; set; }
        public IPluginScript<HandleCommandSet, TickGlobals> TickScript { get; set; }

        public ScriptPluginBuilder(PluginBuilder source) : base(source)
        {
        }
    }
}
