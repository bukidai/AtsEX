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
        /// パッチ先の型のインスタンスを取得します。
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// パッチ先のオリジナルメソッドの戻り値を取得します。
        /// </summary>
        public object ReturnValue { get; }

        /// <summary>
        /// パッチ先のオリジナルメソッドの引数を取得します。
        /// </summary>
        public object[] Args { get; }

        /// <summary>
        /// パッチ先のオリジナルメソッドが実行されるか、あるいは実行されたかを取得します。
        /// </summary>
        public bool RunOriginal { get; }

        internal PatchInvokedEventArgs(object instance, object returnValue, object[] args, bool runOriginal)
        {
            Instance = instance;
            ReturnValue = returnValue;
            Args = args;
            RunOriginal = runOriginal;
        }
    }
}
