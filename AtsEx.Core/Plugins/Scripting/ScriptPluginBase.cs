using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Plugins;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.Plugins.Scripting
{
    internal abstract class ScriptPluginBase : PluginBase, IDisposable
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<ScriptPluginBase>("Core");
        private static readonly string NameText;

        public override string Location { get; } = "";
        public override string Name { get; } = NameText;
        public override string Title { get; } = "";
        public override string Version { get; } = "";
        public override string Description { get; } = "";
        public override string Copyright { get; } = "";

        protected readonly Globals Globals;

        private readonly IPluginScript<Globals> DisposeScript;
        private readonly IPluginScript<ScenarioCreatedGlobals> OnScenarioCreatedScript;
        private readonly IPluginScript<StartedGlobals> OnStartedScript;
        private readonly IPluginScript<TickResult, TickGlobals> TickScript;

        static ScriptPluginBase()
        {
            NameText = Resources.GetString("Name").Value;
        }

        protected ScriptPluginBase(ScriptPluginBuilder builder, PluginType pluginType, bool useAtsExExtensions) : base(builder, pluginType, useAtsExExtensions)
        {
            Location = builder.Location;
            Title = builder.Title;
            Version = builder.Version;
            Description = builder.Description;
            Copyright = builder.Copyright;

            Globals = new Globals(BveHacker);

            DisposeScript = builder.DisposeScript?.GetWithCheckErrors();
            OnScenarioCreatedScript = builder.OnScenarioCreatedScript?.GetWithCheckErrors();
            OnStartedScript = builder.OnStartedScript?.GetWithCheckErrors();
            TickScript = builder.TickScript?.GetWithCheckErrors();

            IPluginScript<Globals> constructorScript = builder.ConstructorScript?.GetWithCheckErrors();
            constructorScript?.Run(Globals);

            BveHacker.ScenarioCreated += OnScenarioCreated;
            App.Started += OnStarted;
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

        public override TickResult Tick(TimeSpan elapsed)
        {
            if (TickScript is null)
            {
                switch (PluginType)
                {
                    case PluginType.VehiclePlugin:
                        return new VehiclePluginTickResult(HandleCommandSet.DoNothing);

                    case PluginType.MapPlugin:
                        return new MapPluginTickResult();
                }
            }

            TickGlobals globals = new TickGlobals(Globals, elapsed);
            IScriptResult<TickResult> result = TickScript.Run(globals) ?? throw new InvalidOperationException(string.Format(Resources.GetString("NoReturnValue").Value, Title, nameof(Tick)));

            return result.ReturnValue;
        }
    }
}
