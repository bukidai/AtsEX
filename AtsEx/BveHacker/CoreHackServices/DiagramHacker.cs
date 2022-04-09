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
    internal interface IDiagramHacker
    {
        void Draw();
    }

    internal sealed class DiagramHacker : CoreHackService, IDiagramHacker
    {
        private ITimePosForm TimePosForm;

        public DiagramHacker(Process targetProcess, ServiceCollection services) : base(targetProcess, services)
        {
            Form formSrc = services.GetService<ISubFormHacker>().TimePosForm;
            TimePosForm = new TimePosForm(formSrc);
        }

        public void Draw() => TimePosForm.Draw();
    }
}
