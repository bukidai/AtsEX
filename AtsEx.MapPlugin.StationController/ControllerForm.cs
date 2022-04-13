using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.MapPlugins.StationController
{
    public partial class ControllerForm : Form
    {
        public ControllerForm()
        {
            InitializeComponent();
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            StationList stations = AtsExPlugin.BveHacker.CurrentScenarioProvider.Route.Stations;
            if (stations.Count == 0) return;
            stations.RemoveAt(stations.Count - 1);

            if (stations.Count == 0)
            {
                RemoveLastStationButton.Enabled = false;
            }
            else
            {
                Station lastStation = stations.Last() as Station;
                lastStation.DepertureTime = int.MaxValue; // 終点の発車時刻は int.MaxValue に設定する
                lastStation.Pass = false;
                lastStation.IsTerminal = true;
            }

            TimeTable timeTable = AtsExPlugin.BveHacker.CurrentScenarioProvider.TimeTable;
            timeTable.NameTexts = new string[stations.Count + 1];
            timeTable.NameTextWidths = new int[stations.Count + 1];
            timeTable.ArrivalTimeTexts = new string[stations.Count + 1];
            timeTable.ArrivalTimeTextWidths = new int[stations.Count + 1];
            timeTable.DepertureTimeTexts = new string[stations.Count + 1];
            timeTable.DepertureTimeTextWidths = new int[stations.Count + 1];
            timeTable.Update();

            AtsExPlugin.BveHacker.UpdateDiagram();
        }
    }
}
