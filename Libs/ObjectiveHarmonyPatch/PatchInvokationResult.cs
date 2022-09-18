using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

namespace ObjectiveHarmonyPatch
{
    /// <summary>
    /// <see cref="HarmonyPatch.Prefix"/>、<see cref="HarmonyPatch.Postfix"/> イベントの結果を表します。
    /// </summary>
    public class PatchInvokationResult
    {
        /// <summary>
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
        /// このパッチをもって、以降のパッチ (とオリジナルメソッド) の実行を中止するかどうかを取得します。動作の仕様の詳細は Harmony のドキュメントをご参照ください。
        /// </summary>
        public bool Cancel { get; }

        /// <summary>
        /// 戻り値の変更を伴う <see cref="PatchInvokationResult"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="returnValue">上書きする戻り値。</param>
        /// <param name="cancel">このパッチをもって、以降のパッチ（とオリジナルメソッド）の実行を中止するかどうか。</param>
        public PatchInvokationResult(object returnValue, bool cancel = false)
        {
            ChangeReturnValue = true;
            ReturnValue = returnValue;
            Cancel = cancel;
        }

        /// <summary>
        /// 戻り値の変更を伴わない <see cref="PatchInvokationResult"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cancel">このパッチをもって、以降のパッチ（とオリジナルメソッド）の実行を中止するかどうか。</param>
        public PatchInvokationResult(bool cancel = false)
        {
            ChangeReturnValue = false;
            Cancel = cancel;
        }
    }
}
