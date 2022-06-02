using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 補助表示を表します。
    /// </summary>
    public class AssistantText : AssistantTextBase
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<AssistantText>();

            ColorGetMethod = members.GetSourcePropertyGetterOf(nameof(Color));
            ColorSetMethod = members.GetSourcePropertySetterOf(nameof(Color));

            TextGetMethod = members.GetSourcePropertyGetterOf(nameof(Text));
            TextSetMethod = members.GetSourcePropertySetterOf(nameof(Text));
        }

        protected AssistantText(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="AssistantText"/> クラスのインスタンス。</returns>
        public static new AssistantText FromSource(object src)
        {
            if (src is null) return null;
            return new AssistantText(src);
        }

        protected static MethodInfo ColorGetMethod;
        protected static MethodInfo ColorSetMethod;
        /// <summary>
        /// 表示するテキストの文字色を取得・設定します。
        /// </summary>
        public Color Color
        {
            get => ColorGetMethod.Invoke(Src, null);
            set => ColorSetMethod.Invoke(Src, new object[] { value });
        }

        protected static MethodInfo TextGetMethod;
        protected static MethodInfo TextSetMethod;
        /// <summary>
        /// 表示するテキストを取得・設定します。
        /// </summary>
        public string Text
        {
            get => TextGetMethod.Invoke(Src, null);
            set => TextSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
