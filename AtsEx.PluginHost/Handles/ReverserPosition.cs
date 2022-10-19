﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AtsEx.PluginHost.Handles
{
    /// <summary>
    /// 逆転器の位置を指定します。
    /// </summary>
    public enum ReverserPosition
    {
        /// <summary>
        /// 後退位置を表します。
        /// </summary>
        B = -1,

        /// <summary>
        /// 中立位置を表します。
        /// </summary>
        N = 0,

        /// <summary>
        /// 前進位置を表します。
        /// </summary>
        F = 1,
    }
}
