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
        private IApp App;
        private IBveHacker BveHacker;

        private AtsEx.PluginHost.IVehicle Vehicle;
        private AtsEx.PluginHost.IRoute Route;

        public StateForm(IApp app, IBveHacker bveHacker, AtsEx.PluginHost.IVehicle vehicle, AtsEx.PluginHost.IRoute route)
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

        private void OnButtonClicked(object sender, EventArgs e)
        {
            IStationList stations = BveHacker.CurrentScenarioProvider.Route.Stations;
            if (stations.Count == 0) return;
            stations.RemoveAt(stations.Count - 1);

            if (stations.Count == 0)
            {
                RemoveLastStationButton.Enabled = false;
            }
            else
            {
                IStation lastStation = stations.Last() as IStation;
                lastStation.DepertureTime = int.MaxValue; // 終点の発車時刻は int.MaxValue に設定する
                lastStation.Pass = false;
                lastStation.IsTerminal = true;
            }

            ITimeTable timeTable = BveHacker.CurrentScenarioProvider.TimeTable;
            timeTable.NameTexts = new string[stations.Count + 1];
            timeTable.NameTextWidths = new int[stations.Count + 1];
            timeTable.ArrivalTimeTexts = new string[stations.Count + 1];
            timeTable.ArrivalTimeTextWidths = new int[stations.Count + 1];
            timeTable.DepertureTimeTexts = new string[stations.Count + 1];
            timeTable.DepertureTimeTextWidths = new int[stations.Count + 1];
            timeTable.Update();

            BveHacker.UpdateDiagram();
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
