using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Mackoy.Bvets;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// すべての補助表示の基本クラスを表します。
    /// </summary>
    public class AssistantTextBase : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<AssistantTextBase>();

            AssistantSettingsGetMethod = members.GetSourcePropertyGetterOf(nameof(AssistantSettings));

            BackgroundColorGetMethod = members.GetSourcePropertyGetterOf(nameof(BackgroundColor));
            BackgroundColorSetMethod = members.GetSourcePropertySetterOf(nameof(BackgroundColor));

            DisplayAreaGetMethod = members.GetSourcePropertyGetterOf(nameof(DisplayArea));

            DrawMethod = members.GetSourceMethodOf(nameof(Draw));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="AssistantTextBase"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected AssistantTextBase(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="AssistantTextBase"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static AssistantTextBase FromSource(object src) => src is null ? null : new AssistantTextBase(src);

        private static MethodInfo AssistantSettingsGetMethod;
        /// <summary>
        /// 補助表示の設定を取得します。
        /// </summary>
        public AssistantSettings AssistantSettings => AssistantSettingsGetMethod.Invoke(Src, null);

        private static MethodInfo BackgroundColorGetMethod;
        private static MethodInfo BackgroundColorSetMethod;
        /// <summary>
        /// 補助表示の背景色を取得・設定します。
        /// </summary>
        public Color BackgroundColor
        {
            get => BackgroundColorGetMethod.Invoke(Src, null);
            set => BackgroundColorSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo DisplayAreaGetMethod;
        /// <summary>
        /// 表示する位置とサイズを表す <see cref="Rectangle"/> を取得します。
        /// </summary>
        public Rectangle DisplayArea => DisplayAreaGetMethod.Invoke(Src, null);

        private static MethodInfo DrawMethod;
        /// <summary>
        /// 補助表示を描画します。
        /// </summary>
        public void Draw()
        {
            DrawMethod.Invoke(Src, null);
        }
    }
}
