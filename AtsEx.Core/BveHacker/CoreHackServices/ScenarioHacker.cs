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
using Automatic9045.AtsEx.PluginHost.BveTypes;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal sealed class ScenarioHacker
    {
        private MainForm MainForm;

        public ScenarioHacker(MainFormHacker mainFormHacker)
        {
            MainForm = mainFormHacker.TargetForm;


            ClassMemberSet timePosFormMembers = BveTypeSet.Instance.GetClassInfoOf<TimePosForm>();
            MethodInfo setScenarioMethod = timePosFormMembers.GetSourceMethodOf(nameof(TimePosForm.SetScenario));

            Harmony harmony = new Harmony("com.automatic9045.atsex.scenario-hacker");
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

#pragma warning disable IDE1006 // 命名スタイル
        private static void SetScenarioPreFix(object[] __args)
#pragma warning restore IDE1006 // 命名スタイル
        {
            Scenario scenario = Scenario.FromSource(__args[0]);
            ScenarioCreated?.Invoke(new ScenarioCreatedEventArgs(scenario));
        }
    }
}
