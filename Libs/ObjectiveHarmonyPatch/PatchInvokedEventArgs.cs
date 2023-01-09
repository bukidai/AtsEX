using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveHarmonyPatch
{
    /// <summary>
    /// <see cref="PatchInvokedEventHandler"/> イベントのデータを提供します。
    /// </summary>
    public class PatchInvokedEventArgs : EventArgs
    {
        /// <summary>
        /// パッチ先の型のインスタンスを取得します。通常の Harmony パッチの __instance パラメータに当たります。
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// パッチ先のオリジナルメソッドの戻り値を取得します。通常の Harmony パッチの __result パラメータに当たります。
        /// </summary>
        public object ReturnValue { get; }

        /// <summary>
        /// パッチ先のオリジナルメソッドの引数を取得します。通常の Harmony パッチの __args パラメータに当たります。
        /// </summary>
        public object[] Args { get; }

        /// <summary>
        /// パッチ先のオリジナルメソッドが実行されるか、あるいは実行されたかを取得します。通常の Harmony パッチの __runOriginal パラメータに当たります。
        /// </summary>
        public bool RunOriginal { get; }

        /// <summary>
        /// 1 つ前の Prefix パッチが <see cref="SkipModes.SkipOriginal"/> を指定したかどうかを取得します。
        /// </summary>
        /// <remarks>
        /// Postfix パッチの場合、またはこれ以前に Prefix パッチが実行されていない場合は <see langword="false"/> を返します。
        /// </remarks>
        public bool SkipOriginal { get; }

        internal PatchInvokedEventArgs(object instance, object returnValue, object[] args, bool runOriginal, bool skipOriginal)
        {
            Instance = instance;
            ReturnValue = returnValue;
            Args = args;
            RunOriginal = runOriginal;
            SkipOriginal = skipOriginal;
        }
    }
}
