using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveHarmonyPatch
{
    /// <summary>
    /// Harmony パッチの種類を表します。
    /// </summary>
    public enum PatchType
    {
        /// <summary>
        /// Prefix パッチを表します。
        /// </summary>
        Prefix,

        /// <summary>
        /// Postfix パッチを表します。
        /// </summary>
        Postfix,
    }
}
