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
    /// レバーサーの位置を指定します。
    /// </summary>
    public enum ReverserPosition
    {
        /// <summary>後進。</summary>
        B = -1,
        /// <summary>中立。</summary>
        N = 0,
        /// <summary>前進。</summary>
        F = 1,
    }
}
