using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

namespace ObjectiveHarmonyPatch
{
    /// <summary>
    /// <see cref="HarmonyPatch.Invoked"/> イベントの結果を表します。
    /// </summary>
    public class PatchInvokationResult
    {
        /// <summa\ry>
        /// パッチ先のオリジナルメソッドの戻り値を上書きするかどうかを取得します。
        /// </summary>
        public bool ChangeReturnValue { get; }

        /// <summary>
        /// パッチ先のオリジナルメソッドの戻り値を上書きする場合は、そのオブジェクトを取得します。上書きしない場合は <see langword="null"/> を返します。
        /// </summary>
        /// <remarks>
        /// このプロパティが <see langword="null"/> を返す場合でも、戻り値を <see langword="null"/> で上書きしている可能性があることに注意してください。<br/>
        /// パッチ先のオリジナルメソッドの戻り値を上書きするかどうかを確認するには <see cref="ChangeReturnValue"/> プロパティを使用してください。
        /// </remarks>
        /// <seealso cref="ChangeReturnValue"/>
        public object ReturnValue { get; }

        /// <summary>
        /// オリジナルメソッドおよびこれ以降のパッチをスキップするかどうかを取得します。
        /// </summary>
        /// <seealso cref="PatchInvokedEventArgs.SkipOriginal"/>
        public SkipModes SkipModes { get; }

        /// <summary>
        /// 戻り値の変更を伴う <see cref="PatchInvokationResult"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="returnValue">上書きする戻り値。</param>
        /// <param name="skipModes">オリジナルメソッドおよびこれ以降のパッチをスキップするかどうか。</param>
        /// <seealso cref="PatchInvokedEventArgs.SkipOriginal"/>
        public PatchInvokationResult(object returnValue, SkipModes skipModes)
        {
            ChangeReturnValue = true;
            ReturnValue = returnValue;
            SkipModes = skipModes;
        }

        /// <summary>
        /// 戻り値の変更を伴わない <see cref="PatchInvokationResult"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="skipModes">オリジナルメソッドおよびこれ以降のパッチをスキップするかどうか。</param>
        /// <seealso cref="PatchInvokedEventArgs.SkipOriginal"/>
        public PatchInvokationResult(SkipModes skipModes)
        {
            ChangeReturnValue = false;
            SkipModes = skipModes;
        }

        /// <summary>
        /// 一切の操作を行わない <see cref="PatchInvokationResult"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="args">1 つ前のパッチの <see cref="SkipModes"/> を継承するための <see cref="PatchInvokedEventArgs"/>。</param>
        /// <seealso cref="PatchInvokedEventArgs.SkipOriginal"/>
        public static PatchInvokationResult DoNothing(PatchInvokedEventArgs args)
            => new PatchInvokationResult(args.SkipOriginal ? SkipModes.SkipOriginal : SkipModes.Continue);
    }
}
