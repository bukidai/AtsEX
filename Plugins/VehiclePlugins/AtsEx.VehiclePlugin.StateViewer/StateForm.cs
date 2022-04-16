using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.VehiclePlugins.StateViewer
{
    public partial class StateForm : Form
    {
        public StateForm()
        {
            InitializeComponent();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is TextBox textBox)
                {
                    switch (textBox.Name)
                    {
                        case nameof(TimeValue):
                            DateTime time;
                            if (DateTime.TryParse(textBox.Text, out time))
                            {
                                AtsExPluginBase.Route.Time = time;
                            }
                            break;

                        case nameof(LocationValue):
                            int location;
                            if (int.TryParse(textBox.Text, out location))
                            {
                                AtsExPluginBase.Vehicle.Location = location;
                            }
                            break;

                        case nameof(SpeedValue):
                            int speed;
                            if (int.TryParse(textBox.Text, out speed))
                            {
                                AtsExPluginBase.Vehicle.Speed = speed;
                            }
                            break;
                    }
                }
            }
        }

        public void Tick()
        {
            if (!TimeValue.Focused) TimeValue.Text = AtsExPluginBase.Route.Time.ToString("HH:mm:ss");
            if (!LocationValue.Focused) LocationValue.Text = AtsExPluginBase.Vehicle.Location.ToString();
            if (!SpeedValue.Focused) SpeedValue.Text = AtsExPluginBase.Vehicle.Speed.ToString();
            DisplaySpeedValue.Text = AtsExPluginBase.Vehicle.DisplaySpeed.ToString();
        }
    }
}
