using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BveTypes;
using BveTypes.ClassWrappers;
using FastMember;
using TypeWrapping;
using ObjectiveHarmonyPatch;

using AtsEx.PluginHost;

namespace AtsEx.BveHackerServices
{
    internal sealed class ScenarioHacker : IDisposable
    {
        private readonly MainForm MainForm;

        private readonly HarmonyPatch LoadPatch;
        private readonly HarmonyPatch DisposePatch;
        private readonly HarmonyPatch InitializeTimeAndLocationMethodPatch;
        private readonly HarmonyPatch InitializeMethodPatch;

        private ScenarioInfo TargetScenarioInfo = null;

        public ScenarioInfo CurrentScenarioInfo
        {
            get => MainForm.CurrentScenarioInfo;
            set => MainForm.CurrentScenarioInfo = value;
        }

        public Scenario CurrentScenario
        {
            get => MainForm.CurrentScenario;
            set => MainForm.CurrentScenario = value;
        }

        public event ScenarioOpenedEventHandler ScenarioOpened;
        public event ScenarioClosedEventHandler ScenarioClosed;
        public event ScenarioCreatedEventHandler ScenarioCreated;

        public ScenarioHacker(MainFormHacker mainFormHacker, BveTypeSet bveTypes)
        {
            MainForm = mainFormHacker.TargetForm;

            ClassMemberSet mainFormMembers = bveTypes.GetClassInfoOf<MainForm>();
            ClassMemberSet scenarioMembers = bveTypes.GetClassInfoOf<Scenario>();

            FastMethod loadMethod = mainFormMembers.GetSourceMethodOf(nameof(BveTypes.ClassWrappers.MainForm.LoadScenario));
            FastMethod disposeMethod = scenarioMembers.GetSourceMethodOf(nameof(Scenario.Dispose));
            FastMethod initializeTimeAndLocationMethod = scenarioMembers.GetSourceMethodOf(nameof(Scenario.InitializeTimeAndLocation));
            FastMethod initializeMethod = scenarioMembers.GetSourceMethodOf(nameof(Scenario.Initialize));

            LoadPatch = CreateAndSetupPatch(loadMethod.Source);
            DisposePatch = CreateAndSetupPatch(disposeMethod.Source);
            InitializeTimeAndLocationMethodPatch = CreateAndSetupPatch(initializeTimeAndLocationMethod.Source, PatchType.Postfix);
            InitializeMethodPatch = CreateAndSetupPatch(initializeMethod.Source, PatchType.Postfix);

            LoadPatch.Invoked += OnLoaded;
            DisposePatch.Invoked += OnDisposed;


            HarmonyPatch CreateAndSetupPatch(MethodBase original, PatchType patchType = PatchType.Prefix)
            {
                HarmonyPatch patch = HarmonyPatch.Patch(nameof(ScenarioHacker), original, patchType);
                return patch;
            }
        }

        private PatchInvokationResult OnLoaded(object sender, PatchInvokedEventArgs e)
        {
            ScenarioInfo scenarioInfo = ScenarioInfo.FromSource(e.Args[0]);
            if (!(scenarioInfo is null) && scenarioInfo != TargetScenarioInfo) ScenarioClosed?.Invoke(EventArgs.Empty);
            TargetScenarioInfo = scenarioInfo;

            ScenarioOpened?.Invoke(new ScenarioOpenedEventArgs(scenarioInfo));

            return PatchInvokationResult.DoNothing(e);
        }

        private PatchInvokationResult OnDisposed(object sender, PatchInvokedEventArgs e)
        {
            if (CurrentScenarioInfo == TargetScenarioInfo)
            {
                TargetScenarioInfo = null;
                ScenarioClosed?.Invoke(EventArgs.Empty);
            }

            return PatchInvokationResult.DoNothing(e);
        }

        public void Dispose()
        {
            LoadPatch.Dispose();
            DisposePatch.Dispose();
            InitializeTimeAndLocationMethodPatch.Dispose();
            InitializeMethodPatch.Dispose();
        }

        public void BeginObserveInitialization()
        {
            InitializeTimeAndLocationMethodPatch.Invoked += OnPatchInvoked;
            InitializeMethodPatch.Invoked += OnPatchInvoked;


            PatchInvokationResult OnPatchInvoked(object sender, PatchInvokedEventArgs e)
            {
                InitializeTimeAndLocationMethodPatch.Invoked -= OnPatchInvoked;
                InitializeMethodPatch.Invoked -= OnPatchInvoked;

                Scenario scenario = Scenario.FromSource(e.Instance);
                ScenarioCreatedEventArgs scenarioCreatedEventArgs = new ScenarioCreatedEventArgs(scenario);
                ScenarioCreated?.Invoke(scenarioCreatedEventArgs);

                return PatchInvokationResult.DoNothing(e);
            }
        }
    }
}
