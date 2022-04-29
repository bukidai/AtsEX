using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Helpers;

namespace Automatic9045.MapPlugins.StationController
{
    public partial class ControllerForm : Form
    {
        public ControllerForm()
        {
            InitializeComponent();

            if (AtsExPluginBase.BveHacker.HasScenarioProviderCreated)
            {
                ResetInput();
            }
            else
            {
                AtsExPluginBase.BveHacker.ScenarioProviderCreated += ScenarioProviderCreated;
            }

            void ScenarioProviderCreated(ScenarioProviderCreatedEventArgs e)
            {
                AtsExPluginBase.BveHacker.ScenarioProviderCreated -= ScenarioProviderCreated;
                ResetInput(e.ScenarioProvider);
            }
        }

        private void ResetInput(ScenarioProvider scenarioProvider = null)
        {
            scenarioProvider = scenarioProvider ?? AtsExPluginBase.BveHacker.ScenarioProvider;

            StationList stations = scenarioProvider.Route.Stations;
            Station lastStation = stations.Count == 0 ? null : stations[stations.Count - 1] as Station;
            
            LocationValue.Text = (lastStation is null ? 0 : lastStation.Location + 500).ToString();
            int arrivalTime = lastStation is null ? 10 * 60 * 60 * 1000 : lastStation.DefaultTime + 2 * 60 * 1000;
            ArrivalTimeValue.Text = arrivalTime.ToTimeText();
            DepertureTimeValue.Text = (arrivalTime + 30 * 1000).ToTimeText();
        }

        private void AddButtonClicked(object sender, EventArgs e)
        {
            StationList stations = AtsExPluginBase.BveHacker.ScenarioProvider.Route.Stations;

            try
            {
                Station newStation = new Station(NameValue.Text)
                {
                    Location = int.Parse(LocationValue.Text),
                    DefaultTime = ArrivalTimeValue.Text.ToTimeMilliseconds(),
                    ArrivalTime = ArrivalTimeValue.Text.ToTimeMilliseconds(),
                    DepertureTime = Pass.Checked ? int.MaxValue : DepertureTimeValue.Text.ToTimeMilliseconds(),
                    Pass = Pass.Checked,
                    IsTerminal = IsTerminal.Checked,
                };
                stations.Add(newStation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("エラーが発生したため駅を追加できませんでした。\n\n詳細：\n\n" + ex.ToString(), "駅追加エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetInput();
                return;
            }

            UpdateStationList();
            ResetInput();
        }

        private void RemoveButtonClicked(object sender, EventArgs e)
        {
            StationList stations = AtsExPluginBase.BveHacker.ScenarioProvider.Route.Stations;
            if (stations.Count == 0) return;
            stations.RemoveAt(stations.Count - 1);

            if (stations.Count == 0)
            {
                RemoveButton.Enabled = false;
            }
            else
            {
                Station lastStation = stations.Last() as Station;
            }

            UpdateStationList();
        }

        private void UpdateStationList()
        {
            StationList stations = AtsExPluginBase.BveHacker.ScenarioProvider.Route.Stations;
            TimeTable timeTable = AtsExPluginBase.BveHacker.ScenarioProvider.TimeTable;
            timeTable.NameTexts = new string[stations.Count + 1];
            timeTable.NameTextWidths = new int[stations.Count + 1];
            timeTable.ArrivalTimeTexts = new string[stations.Count + 1];
            timeTable.ArrivalTimeTextWidths = new int[stations.Count + 1];
            timeTable.DepertureTimeTexts = new string[stations.Count + 1];
            timeTable.DepertureTimeTextWidths = new int[stations.Count + 1];
            timeTable.Update();

            DiagramUpdater.Update();
        }
    }
}
