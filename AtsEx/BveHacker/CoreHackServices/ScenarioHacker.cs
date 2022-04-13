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

namespace Automatic9045.AtsEx.CoreHackServices
{
    internal interface IScenarioHacker
    {
        ScenarioInfo CurrentScenarioInfo { get; set; }
        ScenarioProvider CurrentScenarioProvider { get; set; }
    }

    internal sealed class ScenarioHacker : CoreHackService, IScenarioHacker
    {
        private MainForm MainForm;

        public ScenarioHacker(Process targetProcess, ServiceCollection services) : base(targetProcess, services)
        {
            Form formSrc = Services.GetService<IMainFormHacker>().TargetForm;
            MainForm = MainForm.FromSource(formSrc);


            BveTypeMemberCollection timePosFormMembers = BveTypeCollectionProvider.Instance.GetTypeInfoOf<TimePosForm>();
            MethodInfo setScenarioMethod = timePosFormMembers.GetSourceMethodOf(nameof(TimePosForm.SetScenario));

            Harmony harmony = new Harmony("http://automatic9045.github.io/ns/harmony/atsex/core-hack-services/scenario-hacker");
            harmony.Patch(setScenarioMethod, new HarmonyMethod(typeof(ScenarioHacker), nameof(SetScenarioPreFix)));
        }

        public static event ScenarioProviderCreatedEventHandler ScenarioProviderCreated;

        public ScenarioInfo CurrentScenarioInfo
        {
            get => MainForm.CurrentScenarioInfo;
            set => MainForm.CurrentScenarioInfo = value;
        }

        public ScenarioProvider CurrentScenarioProvider
        {
            get => MainForm.CurrentScenarioProvider;
            set => MainForm.CurrentScenarioProvider = value;
        }

        private static void SetScenarioPreFix(object[] __args)
        {
            ScenarioProvider scenarioProvider = ScenarioProvider.FromSource(__args[0]);
            ScenarioProviderCreated?.Invoke(scenarioProvider);
        }
    }
}
