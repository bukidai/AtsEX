using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.Helpers
{
    public static class DiagramUpdater
    {
        public static void Update()
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

            InstanceStore.BveHacker.TimePosForm.SetScenario(InstanceStore.BveHacker.ScenarioProvider);
        }
    }
}
