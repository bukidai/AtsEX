using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 「シナリオの選択」フォームを表します。
    /// </summary>
    public sealed class ScenarioSelectionForm : ClassWrapper
    {
        private ScenarioSelectionForm(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="ScenarioSelectionForm"/> クラスのインスタンス。</returns>
        public static ScenarioSelectionForm FromSource(object src)
        {
            if (src is null) return null;
            return new ScenarioSelectionForm(src);
        }
    }
}
