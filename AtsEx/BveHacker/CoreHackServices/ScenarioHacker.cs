using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.ClassWrappers;
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
        MainForm MainForm;

        public ScenarioHacker(Process targetProcess, Assembly targetAssembly, ServiceCollection services) : base(targetProcess, targetAssembly, services)
        {
            Form formSource = Services.GetService<IMainFormHacker>().TargetForm;
            MainForm = new MainForm(formSource);
        }

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
    }
}
