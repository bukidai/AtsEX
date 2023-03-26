using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Mackoy.Bvets;

using BveTypes;
using BveTypes.ClassWrappers;
using ObjectiveHarmonyPatch;
using TypeWrapping;
using UnembeddedResources;

using AtsEx.BveHackerServices;
using AtsEx.Handles;
using AtsEx.MapStatements;

using AtsEx.PluginHost;
using AtsEx.PluginHost.LoadErrorManager;
using AtsEx.PluginHost.MapStatements;

namespace AtsEx
{
    internal sealed class BveHacker : IBveHacker, IDisposable
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<BveHacker>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> CannotGetScenario { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static BveHacker()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private readonly HarmonyPatch LoadScenarioPatch;
        private readonly HarmonyPatch UnloadScenarioPatch;

        private readonly StructureSetLifeProlonger StructureSetLifeProlonger;

        public BveHacker(BveTypeSet bveTypes)
        {
            BveTypes = bveTypes;

            MainFormHacker = new MainFormHacker(App.Instance.Process);
            ScenarioHacker = new ScenarioHacker(MainFormHacker, BveTypes);

            StructureSetLifeProlonger = new StructureSetLifeProlonger(this);

            ScenarioHacker.ScenarioCreated += e => PreviewScenarioCreated?.Invoke(e);
            ScenarioHacker.ScenarioCreated += OnScenarioCreated;
            ScenarioHacker.ScenarioCreated += e => ScenarioCreated?.Invoke(e);

            LoadErrorManager = new LoadErrorManager.LoadErrorManager(LoadingProgressForm);

            switch (App.Instance.LaunchMode)
            {
                case LaunchMode.Ats:
                    _MapHeaders = HeaderSet.FromMap(ScenarioInfo.RouteFiles.SelectedFile.Path);
                    break;

                case LaunchMode.InputDevice:
                    ClassMemberSet mainFormMembers = BveTypes.GetClassInfoOf<MainForm>();
                    LoadScenarioPatch = HarmonyPatch.Patch(nameof(BveHacker), mainFormMembers.GetSourceMethodOf(nameof(MainForm.LoadScenario)).Source, PatchType.Prefix);
                    UnloadScenarioPatch = HarmonyPatch.Patch(nameof(BveHacker), mainFormMembers.GetSourceMethodOf(nameof(MainForm.UnloadScenario)).Source, PatchType.Prefix);

                    LoadScenarioPatch.Invoked += (sender, e) =>
                    {
                        ScenarioInfo scenarioInfo = ScenarioInfo.FromSource(e.Args[0]);
                        _MapHeaders = HeaderSet.FromMap(scenarioInfo.RouteFiles.SelectedFile.Path);

                        return new PatchInvokationResult(SkipModes.Continue);
                    };

                    UnloadScenarioPatch.Invoked += (sender, e) =>
                    {
                        _MapHeaders = null;
                        _MapStatements = null;

                        return new PatchInvokationResult(SkipModes.Continue);
                    };

                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            LoadScenarioPatch?.Dispose();
            UnloadScenarioPatch?.Dispose();

            StructureSetLifeProlonger.Dispose();
            ScenarioHacker.Dispose();
        }

        private void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            _MapStatements = StatementSet.Create(e.Scenario.Route.Structures.Repeated, e.Scenario.Route.StructureModels, e.Scenario.Trains);

            NotchInfo notchInfo = e.Scenario.Vehicle.Instruments.Cab.Handles.NotchInfo;

            BrakeHandle brake = BrakeHandle.FromNotchInfo(notchInfo);
            PowerHandle power = PowerHandle.FromNotchInfo(notchInfo);
            Reverser reverser = new Reverser();

            Handles = new PluginHost.Handles.HandleSet(brake, power, reverser);

        }

        public void Tick(TimeSpan elapsed)
        {
            double vehicleLocation = Scenario.LocationManager.Location;
            double preTrainLocation = Scenario.Route.PreTrainObjects.GetPreTrainLocation(Scenario.TimeManager.TimeMilliseconds);

            _MapStatements.Tick(vehicleLocation, preTrainLocation);
        }


        public BveTypeSet BveTypes { get; }


        private readonly MainFormHacker MainFormHacker;
        public IntPtr MainFormHandle => MainFormHacker.TargetFormHandle;
        public Form MainFormSource => MainFormHacker.TargetFormSource;
        public MainForm MainForm => MainFormHacker.TargetForm;

        public Form ScenarioSelectionFormSource => ScenarioSelectionForm.Src;
        public ScenarioSelectionForm ScenarioSelectionForm => MainForm.ScenarioSelectForm;
        public Form LoadingProgressFormSource => LoadingProgressForm.Src;
        public LoadingProgressForm LoadingProgressForm => MainForm.LoadingProgressForm;
        public Form TimePosFormSource => TimePosForm.Src;
        public TimePosForm TimePosForm => MainForm.TimePosForm;
        public Form ChartFormSource => ChartForm.Src;
        public ChartForm ChartForm => MainForm.ChartForm;

        public Preferences Preferences => MainForm.Preferences;
        public KeyProvider KeyProvider => MainForm.KeyProvider;

        public ILoadErrorManager LoadErrorManager { get; }

        public PluginHost.Handles.HandleSet Handles { get; private set; }

#pragma warning disable IDE1006 // 命名スタイル
        public HeaderSet _MapHeaders { get; private set; } = null;
#pragma warning restore IDE1006 // 命名スタイル
        public IHeaderSet MapHeaders => _MapHeaders;

#pragma warning disable IDE1006 // 命名スタイル
        public StatementSet _MapStatements { get; private set; } = null;
#pragma warning restore IDE1006 // 命名スタイル
        public IStatementSet MapStatements => _MapStatements;


        private readonly ScenarioHacker ScenarioHacker;

        public event ScenarioCreatedEventHandler PreviewScenarioCreated;
        public event ScenarioCreatedEventHandler ScenarioCreated;

        public ScenarioInfo ScenarioInfo
        {
            get => ScenarioHacker.CurrentScenarioInfo;
            set => ScenarioHacker.CurrentScenarioInfo = value;
        }
        public Scenario Scenario => ScenarioHacker.CurrentScenario ?? throw new InvalidOperationException(string.Format(Resources.Value.CannotGetScenario.Value, nameof(Scenario)));

        public bool IsScenarioCreated => !(ScenarioHacker.CurrentScenario is null);
    }
}
