using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.BveHackServices
{
    internal interface ILocationHacker
    {
        int Location { set; }
    }

    internal class LocationHacker : BveHackService, ILocationHacker
    {
        protected ToolStripTextBox LocationTextBox { get; }
        protected MethodInfo LocationTextBoxUpdateMethod { get; }

        public int Location
        {
            set
            {
                LocationTextBox.Text = value.ToString();
                UpdateLocation();
            }
        }

        public LocationHacker(BveHacker bveHacker, ServiceCollection services) : base(bveHacker, services)
        {
            LocationTextBox = FindLocationTextBox();
            LocationTextBoxUpdateMethod = FindLocationTextBoxUpdateMethod();
        }

        private ToolStripTextBox FindLocationTextBox()
        {
            ToolStripContainer toolStripContainer = (ToolStripContainer)BveHacker.TimePosForm.Controls.Find("toolStripContainer1", false)[0];
            ToolStrip toolStrip = (ToolStrip)toolStripContainer.TopToolStripPanel.Controls.Find("toolStrip2", false)[0];
            ToolStripTextBox locationTextBox = (ToolStripTextBox)toolStrip.Items.Find("distanceBox", false)[0];

            return locationTextBox;
        }

        private MethodInfo FindLocationTextBoxUpdateMethod()
        {
            Type timePosFormType = BveHacker.TimePosForm.GetType();

            Type[] argTypes = new Type[] { typeof(int) };
            MethodInfo updateMethod = timePosFormType.GetMethod("b", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, argTypes, null);

            return updateMethod;
        }

        protected void UpdateLocation()
        {
            LocationTextBoxUpdateMethod.Invoke(BveHacker.TimePosForm, new object[] { 0 });
        }
    }
}
