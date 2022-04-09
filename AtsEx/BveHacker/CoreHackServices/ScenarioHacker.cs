using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HarmonyLib;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.ClassWrappers;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.CoreHackServices
{
    internal interface IScenarioHacker
    {
        IScenarioInfo CurrentScenarioInfo { get; set; }
        IScenarioProvider CurrentScenarioProvider { get; set; }
    }

    internal sealed class ScenarioHacker : CoreHackService, IScenarioHacker
    {
        private IMainForm MainForm;

        public ScenarioHacker(Process targetProcess, ServiceCollection services) : base(targetProcess, services)
        {
            Form formSrc = Services.GetService<IMainFormHacker>().TargetForm;
            MainForm = new MainForm(formSrc);


            IBveTypeMemberCollection timePosFormMembers = BveTypeCollectionProvider.Instance.GetTypeInfoOf<ITimePosForm>();
            MethodInfo setScenarioMethod = timePosFormMembers.GetSourceMethodOf(nameof(ITimePosForm.SetScenario));

            Harmony harmony = new Harmony("http://automatic9045.github.io/ns/harmony/atsex/core-hack-services/scenario-hacker");
            harmony.Patch(setScenarioMethod, new HarmonyMethod(typeof(ScenarioHacker), nameof(SetScenarioPreFix)));
        }

        public static event ScenarioProviderCreatedEventHandler ScenarioProviderCreated;

        public IScenarioInfo CurrentScenarioInfo
        {
            get => MainForm.CurrentScenarioInfo;
            set => MainForm.CurrentScenarioInfo = value;
        }

        public IScenarioProvider CurrentScenarioProvider
        {
            get => MainForm.CurrentScenarioProvider;
            set => MainForm.CurrentScenarioProvider = value;
        }

        private static void SetScenarioPreFix(object[] __args)
        {
            IScenarioProvider scenarioProvider = new ScenarioProvider(__args[0]);
            ScenarioProviderCreated?.Invoke(scenarioProvider);
        }
    }
}
