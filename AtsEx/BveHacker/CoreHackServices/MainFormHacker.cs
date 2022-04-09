using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.CoreHackServices
{
    internal interface IMainFormHacker
    {
        Form TargetForm { get; }
        dynamic TargetFormAsDynamic { get; }
        IntPtr TargetFormHandle { get; }
        Type TargetFormType { get; }
    }

    internal sealed class MainFormHacker : CoreHackService, IMainFormHacker
    {
        public Form TargetForm { get; }
        public dynamic TargetFormAsDynamic { get; }
        public IntPtr TargetFormHandle { get; }
        public Type TargetFormType { get; }

        public MainFormHacker(Process targetProcess, ServiceCollection services) : base(targetProcess, services)
        {
            TargetFormHandle = TargetProcess.MainWindowHandle;
            TargetForm = (Form)Control.FromHandle(TargetFormHandle);
            TargetFormAsDynamic = TargetForm as dynamic;
            TargetFormType = TargetForm.GetType();
        }
    }
}
