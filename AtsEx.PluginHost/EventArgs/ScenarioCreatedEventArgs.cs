using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AtsEx.PluginHost.ClassWrappers;

namespace AtsEx.PluginHost
{
    /// <summary>
    /// <see cref="BveHacker.PreviewScenarioCreated"/>、<see cref="BveHacker.ScenarioCreated"/> イベントのデータを提供します。
    /// </summary>
    public class ScenarioCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// 生成された <see cref="ClassWrappers.Scenario"/> クラスのインスタンス。
        /// </summary>
        public Scenario Scenario { get; }

        internal ScenarioCreatedEventArgs(Scenario scenario) : base()
        {
            Scenario = scenario;
        }
    }
}
