using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// ブレーキハンドルの位置を指定します。
    /// </summary>
    public enum BrakePosition
    {
        /// <summary>常用ブレーキ位置 (B67)。</summary>
        Service = 0,
        /// <summary>非常ブレーキ位置。</summary>
        Emergency = 1,
        /// <summary>抜き取り位置。</summary>
        Removed = 2,
    }
}
