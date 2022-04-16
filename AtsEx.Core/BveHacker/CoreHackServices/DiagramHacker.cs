using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.CoreHackServices
{
    internal interface IDiagramHacker
    {
        void Update();
    }

    internal sealed class DiagramHacker : CoreHackService, IDiagramHacker
    {
        private Form TimePosFormSource;
        private TimePosForm TimePosForm;

        public DiagramHacker(Process targetProcess, ServiceCollection services) : base(targetProcess, services)
        {
            TimePosFormSource = services.GetService<ISubFormHacker>().TimePosForm;
            TimePosForm = TimePosForm.FromSource(TimePosFormSource);
        }

        public void Update()
        {
            TimePosForm.SetScenario(BveHacker.Instance.CurrentScenarioProvider);
        }
    }
}
