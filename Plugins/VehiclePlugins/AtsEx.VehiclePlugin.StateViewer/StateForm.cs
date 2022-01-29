using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.VehiclePlugins.StateViewer
{
    public partial class StateForm : Form
    {
        private IApp App;
        private IBveHacker BveHacker;

        private IVehicle Vehicle;
        private IRoute Route;

        public StateForm(IApp app, IBveHacker bveHacker, IVehicle vehicle, IRoute route)
        {
            App = app;
            BveHacker = bveHacker;

            Vehicle = vehicle;
            Route = route;

            InitializeComponent();

            App.Elapse += Elapse;
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
                                Route.Time = time;
                            }
                            break;

                        case nameof(LocationValue):
                            int location;
                            if (int.TryParse(textBox.Text, out location))
                            {
                                Vehicle.Location = location;
                            }
                            break;

                        case nameof(SpeedValue):
                            int speed;
                            if (int.TryParse(textBox.Text, out speed))
                            {
                                Vehicle.Speed = speed;
                            }
                            break;
                    }
                }
            }
        }

        private void Elapse(EventArgs e)
        {
            if (!TimeValue.Focused) TimeValue.Text = Route.Time.ToString("HH:mm:ss");
            if (!LocationValue.Focused) LocationValue.Text = Vehicle.Location.ToString();
            if (!SpeedValue.Focused) SpeedValue.Text = Vehicle.Speed.ToString();
            DisplaySpeedValue.Text = Vehicle.DisplaySpeed.ToString();
        }
    }
}
