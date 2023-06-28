using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

using BveTypes.ClassWrappers.Extensions;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// <see cref="MapObjectBase"/> のリストを表します。
    /// </summary>
    /// <seealso cref="SingleStructureList"/>
    /// <seealso cref="StationList"/>
    public class MapObjectList : WrappedList<MapObjectBase>
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<MapObjectList>();

            CurrentIndexGetMethod = members.GetSourcePropertyGetterOf(nameof(CurrentIndex));
            CurrentIndexSetMethod = members.GetSourcePropertySetterOf(nameof(CurrentIndex));

            GoToMethod = members.GetSourceMethodOf(nameof(GoTo));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="MapObjectList"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected MapObjectList(IList src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="MapObjectList"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static MapObjectList FromSource(object src) => src is null ? null : new MapObjectList((IList)src);

        private static FastMethod CurrentIndexGetMethod;
        private static FastMethod CurrentIndexSetMethod;
        /// <summary>
        /// この <see cref="MapObjectList"/> に関連付けられた現在のインデックスを取得・設定します。
        /// </summary>
        /// <remarks>
        /// 通常、この <see cref="MapObjectList"/> において自列車の距離程に対応する値を示します。
        /// </remarks>
        public int CurrentIndex
        {
            get => (int)CurrentIndexGetMethod.Invoke(Src, null);
            set => CurrentIndexSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod GoToMethod;
        /// <summary>
        /// 指定したインデックスへ移動します。
        /// </summary>
        /// <param name="index"></param>
        public void GoTo(int index) => GoToMethod.Invoke(Src, new object[] { index });
    }
}
