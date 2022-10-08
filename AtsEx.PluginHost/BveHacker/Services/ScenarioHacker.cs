using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FastMember;
using TypeWrapping;
using ObjectiveHarmonyPatch;

using Automatic9045.AtsEx.PluginHost.BveTypes;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.BveHackerServices
{
    internal sealed class ScenarioHacker : IDisposable
    {
        private readonly MainForm MainForm;

        private readonly HarmonyPatch InitializeTimeAndLocationMethodPatch;
        private readonly HarmonyPatch InitializeMethodPatch;

        private bool IsScenarioCreatedEventInvoked = false;

        public ScenarioHacker(MainFormHacker mainFormHacker, BveTypeSet bveTypes)
        {
            MainForm = mainFormHacker.TargetForm;

            ClassMemberSet scenarioMembers = bveTypes.GetClassInfoOf<Scenario>();

            FastMethod initializeTimeAndLocationMethod = scenarioMembers.GetSourceMethodOf(nameof(Scenario.InitializeTimeAndLocation));
            FastMethod initializeMethod = scenarioMembers.GetSourceMethodOf(nameof(Scenario.Initialize));

            InitializeTimeAndLocationMethodPatch = CreateAndSetupPatch(initializeTimeAndLocationMethod.Source);
            InitializeMethodPatch = CreateAndSetupPatch(initializeMethod.Source);


            HarmonyPatch CreateAndSetupPatch(MethodBase original)
            {
                HarmonyPatch patch = HarmonyPatch.Patch(original, PatchTypes.Postfix);
                patch.Postfix += OnPatchInvoked;

                return patch;


                PatchInvokationResult OnPatchInvoked(object sender, PatchInvokedEventArgs e)
                {
                    if (IsScenarioCreatedEventInvoked) return new PatchInvokationResult();

                    IsScenarioCreatedEventInvoked = true;

                    Scenario scenario = Scenario.FromSource(e.Instance);
                    ScenarioCreatedEventArgs scenarioCreatedEventArgs = new ScenarioCreatedEventArgs(scenario);
                    ScenarioCreated?.Invoke(scenarioCreatedEventArgs);

                    return new PatchInvokationResult();
                }
            }
        }

        public void Dispose()
        {
            InitializeTimeAndLocationMethodPatch.Dispose();
            InitializeMethodPatch.Dispose();
        }

        public event ScenarioCreatedEventHandler ScenarioCreated;

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
    }
}
