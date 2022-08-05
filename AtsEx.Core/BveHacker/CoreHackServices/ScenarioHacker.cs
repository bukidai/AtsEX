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

        public ScenarioHacker(MainFormHacker mainFormHacker, BveTypeSet bveTypes)
        {
            MainForm = mainFormHacker.TargetForm;

            ClassMemberSet scenarioMembers = bveTypes.GetClassInfoOf<Scenario>();

            MethodInfo initializeTimeAndLocationMethod = scenarioMembers.GetSourceMethodOf(nameof(Scenario.InitializeTimeAndLocation));
            MethodInfo initializeMethod = scenarioMembers.GetSourceMethodOf(nameof(Scenario.Initialize));

            Harmony harmony = new Harmony("com.automatic9045.atsex.scenario-hacker");
            HarmonyMethod patch = new HarmonyMethod(typeof(ScenarioHacker), nameof(SetScenario));

            harmony.Patch(initializeTimeAndLocationMethod, postfix: patch);
            harmony.Patch(initializeMethod, postfix: patch);
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
        private static void SetScenario(object __instance)
#pragma warning restore IDE1006 // 命名スタイル
        {
            Scenario scenario = Scenario.FromSource(__instance);
            ScenarioCreated?.Invoke(new ScenarioCreatedEventArgs(scenario));
        }
    }
}
