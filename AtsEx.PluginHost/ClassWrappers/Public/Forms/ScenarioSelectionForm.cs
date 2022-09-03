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
    public class ScenarioSelectionForm : ClassWrapperBase
    {
        /// <summary>
        /// オリジナル オブジェクトから <see cref="ScenarioSelectionForm"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected ScenarioSelectionForm(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="ScenarioSelectionForm"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static ScenarioSelectionForm FromSource(object src) => src is null ? null : new ScenarioSelectionForm(src);
    }
}
