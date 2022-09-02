using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 「車両物理量」フォームを表します。
    /// </summary>
    public class ChartForm : ClassWrapperBase
    {
        protected ChartForm(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="ChartForm"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static ChartForm FromSource(object src)
        {
            if (src is null) return null;
            return new ChartForm(src);
        }
    }
}
