using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Harmony
{
    /// <summary>
    /// <see cref="ObjectiveHarmonyPatch.Prefix"/>、<see cref="ObjectiveHarmonyPatch.Postfix"/> イベントを処理するメソッドを表します。
    /// </summary>
    /// <param name="sender">イベントのソース。</param>
    /// <param name="e">イベントのデータ。</param>
    /// <returns>イベントの結果。</returns>
    public delegate PatchInvokationResult PatchInvokedEventHandler(object sender, PatchInvokedEventArgs e);
}
