using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

using BveTypes.ClassWrappers.Extensions;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// xy 平面上の、x 座標からそれに対応する y 座標を取得可能なグラフを表します。
    /// </summary>
    public class GraphCurve : WrappedList<GraphCurvePoint>
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<GraphCurve>();

            GetYMethod = members.GetSourceMethodOf(nameof(GetY));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="GraphCurve"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected GraphCurve(IList src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="GraphCurve"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static GraphCurve FromSource(object src) => src is null ? null : new GraphCurve((IList)src);

        private static FastMethod GetYMethod;
        /// <summary>
        /// 指定された x 座標におけるグラフの y 座標を求めます。
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetY(double x) => (double)GetYMethod.Invoke(Src, new object[] { x });
    }
}
