using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveHarmonyPatch
{
    /// <summary>
    /// <see cref="HarmonyPatch.Prefix"/>、<see cref="HarmonyPatch.Postfix"/> イベントを処理するメソッドを表します。
    /// </summary>
    /// <param name="sender">イベントのソース。</param>
    /// <param name="e">イベントのデータ。</param>
    /// <returns>イベントの結果。</returns>
    public delegate PatchInvokationResult PatchInvokedEventHandler(object sender, PatchInvokedEventArgs e);
}
