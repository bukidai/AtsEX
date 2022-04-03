using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.BveHackServices
{
    internal interface ISpeedHacker
    {
        int Speed { get; set; }
    }

    internal class SpeedHacker : BveHackService, ISpeedHacker
    {
        protected ToolStripTextBox VelocityTextBox { get; }
        protected MethodInfo VelocityTextBoxUpdateMethod { get; }

        public int Speed
        {
            get => int.Parse(VelocityTextBox.Text);
            set
            {
                VelocityTextBox.Text = value.ToString();
                UpdateSpeed();
            }
        }

        public SpeedHacker(ServiceCollection services) : base(services)
        {
            VelocityTextBox = FindVelocityTextBox();
            VelocityTextBoxUpdateMethod = FindVelocityTextBoxUpdateMethod();
        }

        private ToolStripTextBox FindVelocityTextBox()
        {
            ToolStripContainer toolStripContainer = (ToolStripContainer)BveHacker.Instance.TimePosForm.Controls.Find("toolStripContainer1", false)[0];
            ToolStrip toolStrip = (ToolStrip)toolStripContainer.TopToolStripPanel.Controls.Find("toolStrip3", false)[0];
            ToolStripTextBox velocityTextBox = (ToolStripTextBox)toolStrip.Items.Find("velocityBox", false)[0];

            return velocityTextBox;
        }

        private MethodInfo FindVelocityTextBoxUpdateMethod()
        {
            Type timePosFormType = BveHacker.Instance.TimePosForm.GetType();

            Type[] argTypes = new Type[] { typeof(int) };
            MethodInfo updateMethod = timePosFormType.GetMethod("a", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, argTypes, null);

            return updateMethod;
        }

        protected void UpdateSpeed()
        {
            VelocityTextBoxUpdateMethod.Invoke(BveHacker.Instance.TimePosForm, new object[] { 0 });
        }
    }
}
