using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;
using AtsEx.Scripting;

namespace AtsEx.Plugins.Scripting
{
    internal sealed class ScriptPluginBuilder : PluginHost.Plugins.PluginBuilder
    {
        private readonly PluginLoader CreatedBy;

        public string Location { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }

        public IPluginScript<Globals> ConstructorScript { get; set; }
        public IPluginScript<Globals> DisposeScript { get; set; }
        public IPluginScript<ScenarioCreatedGlobals> OnScenarioCreatedScript { get; set; }
        public IPluginScript<StartedGlobals> OnStartedScript { get; set; }
        public IPluginScript<TickResult, TickGlobals> TickScript { get; set; }

        protected override event AllExtensionsLoadedEventHandler AllExtensionsLoaded;
        protected override event AllPluginsLoadedEventHandler AllPluginsLoaded;

        public ScriptPluginBuilder(PluginBuilder source) : base(source)
        {
            CreatedBy = source.CreatedBy;

            CreatedBy.AllExtensionsLoaded += e => source.GetAllExtensionsLoaded()?.Invoke(e);
            CreatedBy.AllPluginsLoaded += e => source.GetAllPluginsLoaded()?.Invoke(e);
        }
    }
}
