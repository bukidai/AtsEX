using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// 補助表示を表します。
    /// </summary>
    public class AssistantText : AssistantTextBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<AssistantText>();

            ColorGetMethod = members.GetSourcePropertyGetterOf(nameof(Color));
            ColorSetMethod = members.GetSourcePropertySetterOf(nameof(Color));

            TextGetMethod = members.GetSourcePropertyGetterOf(nameof(Text));
            TextSetMethod = members.GetSourcePropertySetterOf(nameof(Text));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="AssistantText"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected AssistantText(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="AssistantText"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static new AssistantText FromSource(object src) => src is null ? null : new AssistantText(src);

        private static FastMethod ColorGetMethod;
        private static FastMethod ColorSetMethod;
        /// <summary>
        /// 表示するテキストの文字色を取得・設定します。
        /// </summary>
        public Color Color
        {
            get => ColorGetMethod.Invoke(Src, null);
            set => ColorSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod TextGetMethod;
        private static FastMethod TextSetMethod;
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
