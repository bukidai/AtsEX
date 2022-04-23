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
            ScenarioProvider scenarioProvider = AtsExPluginBase.BveHacker.CurrentScenarioProvider;

            if (e.KeyCode == Keys.Enter)
            {
                if (sender is TextBox textBox)
                {
                    switch (textBox.Name)
                    {
                        case nameof(TimeValue):
                            TimeSpan time;
                            if (TimeSpan.TryParse(textBox.Text, out time))
                            {
                                scenarioProvider.TimeManager.SetTime((int)time.TotalMilliseconds);
                            }
                            break;
                            
                        case nameof(LocationValue):
                            double location;
                            if (double.TryParse(textBox.Text, out location))
                            {
                                scenarioProvider.LocationManager.SetLocation(location, true);
                            }
                            break;

                        case nameof(SpeedValue):
                            double speed;
                            if (double.TryParse(textBox.Text, out speed))
                            {
                                scenarioProvider.LocationManager.SetSpeed(speed / 3.6);
                            }
                            break;
                    }
                }
            }
        }

        public void Tick()
        {
            ScenarioProvider scenarioProvider = AtsExPluginBase.BveHacker.CurrentScenarioProvider;

            if (!TimeValue.Focused) TimeValue.Text = TimeSpan.FromMilliseconds(scenarioProvider.TimeManager.TimeMilliseconds).ToString(@"hh\:mm\:ss");
            if (!LocationValue.Focused) LocationValue.Text = scenarioProvider.LocationManager.Location.ToString("F");
            if (!SpeedValue.Focused) SpeedValue.Text = (scenarioProvider.LocationManager.SpeedMeterPerSecond * 3.6).ToString("F");
        }
    }
}
