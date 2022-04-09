using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.ClassWrappers;

namespace Automatic9045.AtsEx.CoreHackServices
{
    /// <summary>
    /// 子フォーム類のインスタンスを取得します。<br />
    /// 設定、バージョン情報については、表示の度にインスタンスを生成しているため取得できません。
    /// </summary>
    internal interface ISubFormHacker
    {
        Form ScenarioSelectForm { get; }
        Form LoadingProgressForm { get; }

        Form TimePosForm { get; }
        Form ChartForm { get; }
    }

    internal sealed class SubFormHacker : CoreHackService, ISubFormHacker
    {
        public Form ScenarioSelectForm { get; }
        public Form LoadingProgressForm { get; }

        public Form TimePosForm { get; }
        public Form ChartForm { get; }

        public SubFormHacker(Process targetProcess, ServiceCollection services) : base(targetProcess, services)
        {
            Form mainFormSrc = Services.GetService<IMainFormHacker>().TargetForm;
            MainForm mainForm = new MainForm(mainFormSrc);

            ScenarioSelectForm = mainForm.ScenarioSelectForm;
            LoadingProgressForm = mainForm.LoadingProgressForm;

            TimePosForm = mainForm.TimePosForm;
            ChartForm = mainForm.ChartForm;
        }
    }
}
