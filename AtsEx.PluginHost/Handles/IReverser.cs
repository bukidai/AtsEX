using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Handles
{
    /// <summary>
    /// 逆転器を表します。
    /// </summary>
    public interface IReverser
    {
        /// <summary>
        /// 現在の位置を取得します。
        /// </summary>
        ReverserPosition Position { get; }
    }
}
