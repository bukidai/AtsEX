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
    /// 自列車の制御段を表します。
    /// </summary>
    public class VehicleStep : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<VehicleStep>();

            CurvesGetMethod = members.GetSourcePropertyGetterOf(nameof(Curves));

            ForceGetMethod = members.GetSourcePropertyGetterOf(nameof(Force));
            MaxForceGetMethod = members.GetSourcePropertyGetterOf(nameof(MaxForce));
            CurrentGetMethod = members.GetSourcePropertyGetterOf(nameof(Current));
            MaxCurrentGetMethod = members.GetSourcePropertyGetterOf(nameof(MaxCurrent));
            NoLoadCurrentGetMethod = members.GetSourcePropertyGetterOf(nameof(NoLoadCurrent));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="VehicleStep"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected VehicleStep(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="VehicleStep"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static VehicleStep FromSource(object src) => src is null ? null : new VehicleStep(src);

        private static FastMethod CurvesGetMethod;
        /// <summary>
        /// テーブルの一覧を取得します。
        /// </summary>
        public WrappedSortedList<string, GraphCurve> Curves
        {
            get
            {
                IDictionary dictionarySrc = CurvesGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, GraphCurve>(dictionarySrc);
            }
        }

        private static FastMethod ForceGetMethod;
        /// <summary>
        /// 空車時の引張力を指定する引張力テーブルを取得します。
        /// </summary>
        public GraphCurve Force => GraphCurve.FromSource(ForceGetMethod.Invoke(Src, null));

        private static FastMethod MaxForceGetMethod;
        /// <summary>
        /// 応荷重制御で増加できる最大の引張力を指定する引張力テーブルを取得します。
        /// </summary>
        public GraphCurve MaxForce => GraphCurve.FromSource(MaxForceGetMethod.Invoke(Src, null));

        private static FastMethod CurrentGetMethod;
        /// <summary>
        /// 空車時の主電動機電流を指定する電流テーブルを取得します。
        /// </summary>
        public GraphCurve Current => GraphCurve.FromSource(CurrentGetMethod.Invoke(Src, null));

        private static FastMethod MaxCurrentGetMethod;
        /// <summary>
        /// 応荷重制御で増加できる最大の主電動機電流を指定する電流テーブルを取得します。
        /// </summary>
        public GraphCurve MaxCurrent => GraphCurve.FromSource(MaxCurrentGetMethod.Invoke(Src, null));

        private static FastMethod NoLoadCurrentGetMethod;
        /// <summary>
        /// 引張力が 0 となる主電動機電流を指定する電流テーブルを取得します。
        /// </summary>
        public GraphCurve NoLoadCurrent => GraphCurve.FromSource(NoLoadCurrentGetMethod.Invoke(Src, null));
    }
}
