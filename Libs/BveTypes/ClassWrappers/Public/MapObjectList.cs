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

            ObjectPassedEvent = members.GetSourceEventOf(nameof(ObjectPassed));

            GoToAndGetCurrentMethod = members.GetSourceMethodOf(nameof(GoToAndGetCurrent));
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

        private static FastEvent ObjectPassedEvent;
        /// <summary>
        /// <see cref="CurrentIndex"/> の値が変更されたときに発生します。
        /// </summary>
        public event ObjectPassedEventHandler ObjectPassed
        {
            add => ObjectPassedEvent.Add(Src, value);
            remove => ObjectPassedEvent.Remove(Src, value);
        }
        /// <summary>
        /// <see cref="ObjectPassed"/> イベントを実行します。
        /// </summary>
        /// <param name="args">自列車が通過したマップ オブジェクト。</param>
        public void ObjectPassed_Invoke(ObjectPassedEventArgs args) => ObjectPassedEvent.Invoke(Src, new object[] { Src, args.Src });

        private static FastMethod GoToAndGetCurrentMethod;
        /// <summary>
        /// コレクションの指定した距離程に対応する要素へ移動し、それを取得します。
        /// </summary>
        /// <param name="location">移動先の距離程 [m]。</param>
        public MapObjectBase GoToAndGetCurrent(double location)
        {
            object src = GoToAndGetCurrentMethod.Invoke(Src, new object[] { location });
            return src is null ? null : (MapObjectBase)CreateFromSource(src);
        }

        private static FastMethod GoToMethod;
        /// <summary>
        /// 指定したインデックスへ移動します。
        /// </summary>
        /// <param name="index">移動先のインデックス。</param>
        public void GoTo(int index) => GoToMethod.Invoke(Src, new object[] { index });


        public delegate void ObjectPassedEventHandler(object sender, ObjectPassedEventArgs e);
    }
}
