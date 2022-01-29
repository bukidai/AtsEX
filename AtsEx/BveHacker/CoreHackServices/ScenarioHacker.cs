using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.ClassWrappers;

namespace Automatic9045.AtsEx.CoreHackServices
{
    internal interface IScenarioHacker
    {
        ScenarioInfo CurrentScenarioInfo { get; set; }
        ScenarioProvider CurrentScenarioProvider { get; set; }
    }

    internal sealed class ScenarioHacker : CoreHackService, IScenarioHacker
    {
        private FieldInfo CurrentScenarioInfoField;
        private FieldInfo CurrentScenarioProviderField;

        [WillRefactor]
        public ScenarioHacker(Process targetProcess, Assembly targetAssembly, ServiceCollection services) : base(targetProcess, targetAssembly, services)
        {
            CurrentScenarioInfoField = Services.GetService<IMainFormHacker>().TargetFormType.GetField("q", BindingFlags.Instance | BindingFlags.NonPublic);
            CurrentScenarioProviderField = Services.GetService<IMainFormHacker>().TargetFormType.GetField("n", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public ScenarioInfo CurrentScenarioInfo
        {
            get
            {
                dynamic src = CurrentScenarioInfoField.GetValue(Services.GetService<IMainFormHacker>().TargetForm);
                return new ScenarioInfo(TargetAssembly, src);
            }
            set => CurrentScenarioInfoField.SetValue(Services.GetService<IMainFormHacker>().TargetForm, value.Src);
        }

        public ScenarioProvider CurrentScenarioProvider
        {
            get
            {
                dynamic src = CurrentScenarioProviderField.GetValue(Services.GetService<IMainFormHacker>().TargetForm);
                return new ScenarioProvider(src);
            }
            set => CurrentScenarioProviderField.SetValue(Services.GetService<IMainFormHacker>().TargetForm, value.Src);
        }
    }
}
