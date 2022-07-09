using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.Plugins.Scripting.CSharp
{
    internal class CSharpScriptPluginBuilder : PluginBuilder
    {
        public string Location { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }

        public PluginScript<Globals> ConstructorScript { get; set; }
        public PluginScript<Globals> DisposeScript { get; set; }
        public PluginScript<ScenarioCreatedGlobals> OnScenarioCreatedScript { get; set; }
        public PluginScript<StartedGlobals> OnStartedScript { get; set; }
        public PluginScript<TickGlobals> TickScript { get; set; }

        public CSharpScriptPluginBuilder(PluginBuilder source) : base(source.App)
        {
            if (!(source.BveHacker is null)) UseAtsExExtensions(source.BveHacker);
        }
    }
}
