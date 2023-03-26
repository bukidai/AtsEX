using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost
{
    /// <summary>
    /// AtsEX 本体の呼出モードを表します。
    /// </summary>
    public enum LaunchMode
    {
        /// <summary>
        /// AtsEx.Caller から ATS プラグインとして起動されたことを表します。
        /// </summary>
        Ats,

        /// <summary>
        /// AtsEx.Caller.InputDevice から入力デバイスプラグインとして起動されたことを表します。
        /// </summary>
        InputDevice,
    }
}
