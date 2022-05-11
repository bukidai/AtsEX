using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HarmonyLib;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal sealed class ScenarioHacker
    {
        private MainForm MainForm;

        public ScenarioHacker(MainFormHacker mainFormHacker)
        {
            MainForm = mainFormHacker.TargetForm;


            ClassMemberCollection timePosFormMembers = BveTypeCollectionProvider.Instance.GetClassInfoOf<TimePosForm>();
            MethodInfo setScenarioMethod = timePosFormMembers.GetSourceMethodOf(nameof(TimePosForm.SetScenario));

            Harmony harmony = new Harmony("http://automatic9045.github.io/ns/harmony/atsex/core-hack-services/scenario-hacker");
            harmony.Patch(setScenarioMethod, new HarmonyMethod(typeof(ScenarioHacker), nameof(SetScenarioPreFix)));
        }

        public static event ScenarioCreatedEventHandler ScenarioCreated;

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

        private static void SetScenarioPreFix(object[] __args)
        {
            Scenario scenario = Scenario.FromSource(__args[0]);
            ScenarioCreated?.Invoke(new ScenarioCreatedEventArgs(scenario));
        }
    }
}
