using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost
{
    /// <summary>
    /// 定速制御の状態を指定します。
    /// </summary>
    public enum ConstantSpeedState
    {
        /// <summary>現在の状態を維持します。</summary>
        Continue = 0,
        /// <summary>定速制御を起動します。</summary>
        Enable = 1,
        /// <summary>定速制御を停止します。</summary>
        Disable = 2,
    }
}
