using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Panels.Native
{
    /// <summary>
    /// ATS プラグインによって制御可能な運転台パネルの状態量 (例えば「ats12」など、subjectKey が「ats」から始まる状態量) を表します。
    /// </summary>
    /// <typeparam name="T">状態量の型。</typeparam>
    public interface IAtsPanelValue<T> : IDisposable
    {
        /// <summary>
        /// 状態量を取得・設定します。
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// <see cref="Value"/> を BVE 本体へ渡す整数値にシリアライズした結果を取得します。
        /// </summary>
        int SerializedValue { get; }

        /// <summary>
        /// <see cref="Value"/> の値が変更されたときに発生します。
        /// </summary>
        event EventHandler ValueChanged;
    }
}
