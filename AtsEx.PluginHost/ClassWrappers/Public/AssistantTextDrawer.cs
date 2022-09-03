using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 補助表示を描画するための機能を提供します。
    /// </summary>
    public class AssistantTextDrawer : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<AssistantTextDrawer>();

            AssistantTextsGetMethod = members.GetSourcePropertyGetterOf(nameof(AssistantTexts));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="AssistantTextDrawer"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected AssistantTextDrawer(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="AssistantText"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static AssistantTextDrawer FromSource(object src) => src is null ? null : new AssistantTextDrawer(src);

        private static MethodInfo AssistantTextsGetMethod;
        /// <summary>
        /// 補助表示の一覧を取得します。
        /// </summary>
        public WrappedList<AssistantTextBase> AssistantTexts => WrappedList<AssistantTextBase>.FromSource(AssistantTextsGetMethod.Invoke(Src, null));
    }
}
