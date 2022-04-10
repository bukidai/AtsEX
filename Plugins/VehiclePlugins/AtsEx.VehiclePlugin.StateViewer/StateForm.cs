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

            AtsExPlugin.App.Elapse += Elapse;
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
                                AtsExPlugin.Route.Time = time;
                            }
                            break;

                        case nameof(LocationValue):
                            int location;
                            if (int.TryParse(textBox.Text, out location))
                            {
                                AtsExPlugin.Vehicle.Location = location;
                            }
                            break;

                        case nameof(SpeedValue):
                            int speed;
                            if (int.TryParse(textBox.Text, out speed))
                            {
                                AtsExPlugin.Vehicle.Speed = speed;
                            }
                            break;
                    }
                }
            }
        }

        private void Elapse(EventArgs e)
        {
            if (!TimeValue.Focused) TimeValue.Text = AtsExPlugin.Route.Time.ToString("HH:mm:ss");
            if (!LocationValue.Focused) LocationValue.Text = AtsExPlugin.Vehicle.Location.ToString();
            if (!SpeedValue.Focused) SpeedValue.Text = AtsExPlugin.Vehicle.Speed.ToString();
            DisplaySpeedValue.Text = AtsExPlugin.Vehicle.DisplaySpeed.ToString();
        }
    }
}
