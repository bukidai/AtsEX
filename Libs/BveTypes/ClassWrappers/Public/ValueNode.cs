using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// <typeparamref name="T"/> 型のデータが一つ記憶可能な <see cref="MapObjectBase"/> を表します。
    /// </summary>
    public class ValueNode<T> : MapObjectBase
    {
        private static BveTypeSet BveTypes = null;

        private static void LoadMembers()
        {
            ClassMemberSet members = BveTypes.GetClassInfoOf<ValueNode<T>>();

            ValueGetMethod = members.GetSourcePropertyGetterOf(nameof(Value));
            ValueSetMethod = members.GetSourcePropertySetterOf(nameof(Value));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="ValueNode{T}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected ValueNode(object src) : base(src)
        {
            if (BveTypes is null)
            {
                BveTypes = ClassWrapperInitializer.LazyInitialize();
                LoadMembers();
            }
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="ValueNode{T}"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static ValueNode<T> FromSource(object src) => src is null ? null : new ValueNode<T>(src);

        private static FastMethod ValueGetMethod;
        private static FastMethod ValueSetMethod;
        /// <summary>
        /// この <see cref="ValueNode{T}"/> が格納しているデータを取得・設定します。
        /// </summary>
        public T Value
        {
            get => ValueGetMethod.Invoke(Src, null);
            set => ValueSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
