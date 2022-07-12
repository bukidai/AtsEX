using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Plugins;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.Plugins.Scripting.CSharp
{
    internal class CSharpScriptPlugin : PluginBase, IDisposable
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<CSharpScriptPlugin>("Core");
        private static readonly string NameText;

        public override string Location { get; } = "";
        public override string Name { get; } = NameText;
        public override string Title { get; } = "";
        public override string Version { get; } = "";
        public override string Description { get; } = "";
        public override string Copyright { get; } = "";

        protected readonly Globals Globals;

        private readonly PluginScript<Globals> DisposeScript;
        private readonly PluginScript<ScenarioCreatedGlobals> OnScenarioCreatedScript;
        private readonly PluginScript<StartedGlobals> OnStartedScript;
        private readonly PluginScript<TickGlobals> TickScript;

        static CSharpScriptPlugin()
        {
            NameText = Resources.GetString("Name").Value;
        }

        protected CSharpScriptPlugin(CSharpScriptPluginBuilder builder, PluginType pluginType, bool useAtsExExtensions) : base(builder, pluginType, useAtsExExtensions)
        {
            Location = builder.Location;
            Title = builder.Title;
            Version = builder.Version;
            Description = builder.Description;
            Copyright = builder.Copyright;

            Globals = new Globals(App, BveHacker);

            DisposeScript = builder.DisposeScript?.GetWithCheckCompilationErrors();
            OnScenarioCreatedScript = builder.OnScenarioCreatedScript?.GetWithCheckCompilationErrors();
            OnStartedScript = builder.OnStartedScript?.GetWithCheckCompilationErrors();
            TickScript = builder.TickScript?.GetWithCheckCompilationErrors();

            PluginScript<Globals> constructorScript = builder.ConstructorScript?.GetWithCheckCompilationErrors();
            constructorScript?.Run(Globals);

            BveHacker.ScenarioCreated += OnScenarioCreated;
            App.Started += OnStarted;
        }

        public static CSharpScriptPlugin FromPackage(PluginBuilder builder, PluginType pluginType, ScriptPluginPackage package)
        {
            CSharpScriptPluginBuilder newBuilder = new CSharpScriptPluginBuilder(builder)
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
                TickScript = package.TickScriptPath is null ?                           null : PluginScript<TickGlobals>.LoadFrom(package.TickScriptPath),
            };

            return new CSharpScriptPlugin(newBuilder, pluginType, !(newBuilder.BveHacker is null));
        }

        public void Dispose() => DisposeScript?.Run(Globals);

        private void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            ScenarioCreatedGlobals globals = new ScenarioCreatedGlobals(Globals, e);
            OnScenarioCreatedScript?.Run(globals);
        }

        private void OnStarted(StartedEventArgs e)
        {
            StartedGlobals globals = new StartedGlobals(Globals, e);
            OnStartedScript?.Run(globals);
        }

        public override void Tick(TimeSpan elapsed)
        {
            TickGlobals globals = new TickGlobals(Globals, elapsed);
            TickScript?.Run(globals);
        }
    }
}
