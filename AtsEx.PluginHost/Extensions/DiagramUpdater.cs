using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.Extensions
{
    /// <summary>
    /// 時刻表、ダイヤグラムなどの行路に関わるオブジェクトの更新機能を提供します。
    /// </summary>
    public static class DiagramUpdater
    {
        /// <summary>
        /// 時刻表と「時刻と位置」フォーム内のダイヤグラムの表示を最新の設定に更新します。
        /// </summary>
        /// <param name="bveHacker">更新に使用する <see cref="BveHacker"/>。</param>
        public static void UpdateDiagram(this BveHacker bveHacker)
        {
            StationList stations = bveHacker.Scenario.Route.Stations;
            TimeTable timeTable = bveHacker.Scenario.TimeTable;

            timeTable.NameTexts = new string[stations.Count + 1];
            timeTable.NameTextWidths = new int[stations.Count + 1];
            timeTable.ArrivalTimeTexts = new string[stations.Count + 1];
            timeTable.ArrivalTimeTextWidths = new int[stations.Count + 1];
            timeTable.DepertureTimeTexts = new string[stations.Count + 1];
            timeTable.DepertureTimeTextWidths = new int[stations.Count + 1];
            timeTable.Update();

            bveHacker.TimePosForm.SetScenario(bveHacker.Scenario);
        }
    }
}
