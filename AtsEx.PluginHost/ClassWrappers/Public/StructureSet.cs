using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// Structure マップ要素、Repeater マップ要素で設置されたストラクチャーの情報を提供します。
    /// </summary>
    public sealed class StructureSet : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<StructureSet>();

            DrawLimitLocationGetMethod = members.GetSourcePropertyGetterOf(nameof(DrawLimitLocation));
            DrawLimitLocationSetMethod = members.GetSourcePropertySetterOf(nameof(DrawLimitLocation));

            RepeatedGetMethod = members.GetSourcePropertyGetterOf(nameof(Repeated));

            PutGetMethod = members.GetSourcePropertyGetterOf(nameof(Put));

            PutBetweenGetMethod = members.GetSourcePropertyGetterOf(nameof(PutBetween));

            SignalsGetMethod = members.GetSourcePropertyGetterOf(nameof(Signals));
        }

        private StructureSet(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="StructureSet"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static StructureSet FromSource(object src) => src is null ? null : new StructureSet(src);

        private static MethodInfo DrawLimitLocationGetMethod;
        private static MethodInfo DrawLimitLocationSetMethod;
        /// <summary>
        /// ストラクチャーが設置される限界の距離程 [m] を取得・設定します。通常は最後の駅の 10km 先の位置になります。
        /// </summary>
        /// <remarks>
        /// この数値を変更しても BVE には反映されません。
        /// </remarks>
        public double DrawLimitLocation
        {
            get => DrawLimitLocationGetMethod.Invoke(Src, null);
            set => DrawLimitLocationSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo RepeatedGetMethod;
        /// <summary>
        /// Repeater マップ要素で設置されたストラクチャーを取得します。
        /// </summary>
        /// <value>キーが RepeaterKey で指定した連続ストラクチャーー名、値が設置するストラクチャーのリストを表す <see cref="MapObjectList"/> の <see cref="WrappedSortedList{string, MapObjectList}"/>。</value>
        public WrappedSortedList<string, MapObjectList> Repeated
        {
            get
            {
                IDictionary dictionarySrc = RepeatedGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, MapObjectList>(dictionarySrc);
            }
        }

        private static MethodInfo PutGetMethod;
        /// <summary>
        /// Structure[].Put ステートメント、Structure[].Put0 ステートメントで設置されたストラクチャーを取得します。
        /// </summary>
        public SingleStructureList Put => SingleStructureList.FromSource(PutGetMethod.Invoke(Src, null));

        private static MethodInfo PutBetweenGetMethod;
        /// <summary>
        /// Structure[].PutBetween ステートメントで設置されたストラクチャーを取得します。
        /// </summary>
        public SingleStructureList PutBetween => SingleStructureList.FromSource(PutBetweenGetMethod.Invoke(Src, null));

        private static MethodInfo SignalsGetMethod;
        /// <summary>
        /// Signal[].Put ステートメントで設置された信号ストラクチャーを取得します。
        /// </summary>
        public SingleStructureList Signals => SingleStructureList.FromSource(SignalsGetMethod.Invoke(Src, null));
    }
}
