using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.BveHackServices
{
    internal interface ITimeHacker
    {
        DateTime Time { get; set; }
    }

    internal class TimeHacker : BveHackService, ITimeHacker
    {
        protected ToolStripTextBox TimeTextBox { get; }
        protected MethodInfo TimeTextBoxUpdateMethod { get; }

        public DateTime Time
        {
            get => DateTime.Parse(TimeTextBox.Text);
            set
            {
                TimeTextBox.Text = value.ToString("HH:mm:ss");
                UpdateTime();
            }
        }

        public TimeHacker(BveHacker bveHacker, ServiceCollection services) : base(bveHacker, services)
        {
            TimeTextBox = FindTimeTextBox();
            TimeTextBoxUpdateMethod = FindTimeTextBoxUpdateMethod();
        }

        private ToolStripTextBox FindTimeTextBox()
        {
            ToolStripContainer toolStripContainer = (ToolStripContainer)BveHacker.TimePosForm.Controls.Find("toolStripContainer1", false)[0];
            ToolStrip toolStrip = (ToolStrip)toolStripContainer.TopToolStripPanel.Controls.Find("toolStrip1", false)[0];
            ToolStripTextBox velocityTextBox = (ToolStripTextBox)toolStrip.Items.Find("timeBox", false)[0];

            return velocityTextBox;
        }

        private MethodInfo FindTimeTextBoxUpdateMethod()
        {
            Type timePosFormType = BveHacker.TimePosForm.GetType();

            Type[] argTypes = new Type[] { typeof(int) };
            MethodInfo updateMethod = timePosFormType.GetMethod("c", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, argTypes, null);

            return updateMethod;
        }

        protected void UpdateTime()
        {
            TimeTextBoxUpdateMethod.Invoke(BveHacker.TimePosForm, new object[] { 0 });
        }
    }
}
