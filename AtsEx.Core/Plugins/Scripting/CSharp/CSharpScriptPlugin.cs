using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.Plugins.Scripting.CSharp
{
    internal sealed class CSharpScriptPlugin : ScriptPluginBase
    {
        protected CSharpScriptPlugin(ScriptPluginBuilder builder, PluginType pluginType) : base(builder, pluginType, true)
        {
        }

        public static CSharpScriptPlugin FromPackage(PluginBuilder builder, PluginType pluginType, ScriptPluginPackage package)
        {
            ScriptPluginBuilder newBuilder = new ScriptPluginBuilder(builder)
            {
                Location = package.Location,
                Title = package.Title,
                Version = package.Version,
                Description = package.Description,
                Copyright = package.Copyright,

                ConstructorScript = package.ConstructorScriptPath is null ?             null : PluginScript<Globals>.LoadFrom(package.ConstructorScriptPath),
                DisposeScript = package.DisposeScriptPath is null ?                     null : PluginScript<Globals>.LoadFrom(package.DisposeScriptPath),
                OnScenarioCreatedScript = package.OnScenarioCreatedScriptPath is null ? null : PluginScript<ScenarioCreatedGlobals>.LoadFrom(package.OnScenarioCreatedScriptPath),
                OnStartedScript = package.OnStartedScriptPath is null ?                 null : PluginScript<StartedGlobals>.LoadFrom(package.OnStartedScriptPath),
                TickScript = package.TickScriptPath is null ?                           null : PluginScript<HandleCommandSet, TickGlobals>.LoadFrom(package.TickScriptPath),
            };

            return new CSharpScriptPlugin(newBuilder, pluginType);
        }
    }
}
