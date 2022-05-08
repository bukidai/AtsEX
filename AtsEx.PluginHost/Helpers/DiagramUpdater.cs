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
    /// <summary>
    /// 時刻表、ダイヤグラムなどの行路に関わるオブジェクトの更新機能を提供します。
    /// </summary>
    public static class DiagramUpdater
    {
        /// <summary>
        /// 時刻表と「時刻と位置」フォーム内のダイヤグラムの表示を最新の設定に更新します。
        /// </summary>
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
