using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveHarmonyPatch
{
    /// <summary>
    /// オリジナルメソッドおよびパッチをスキップするモードを指定します。
    /// </summary>
    [Flags]
    public enum SkipModes
    {
        /// <summary>
        /// オリジナルメソッドおよびこれ以降のパッチはスキップされません。
        /// </summary>
        Continue = 0,

        /// <summary>
        /// これ以降のパッチはスキップされます。
        /// </summary>
        SkipPatches = 1,

        /// <summary>
        /// オリジナルメソッドはスキップされます。Prefix パッチの場合のみ有効です。
        /// </summary>
        /// <remarks>
        /// <see cref="SkipPatches"/> を指定しない場合、これ以降のパッチでの指定が優先されます。
        /// </remarks>
        SkipOriginal = 2,
    }
}
