using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// ブレーキシリンダーの圧力調整弁を表します。
    /// </summary>
    public class CarBc : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<CarBc>();

            BasicBrakeGetMethod = members.GetSourcePropertyGetterOf(nameof(BasicBrake));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="CarBc"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected CarBc(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="CarBc"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static CarBc FromSource(object src) => src is null ? null : new CarBc(src);

        private static FastMethod BasicBrakeGetMethod;
        /// <summary>
        /// 基礎ブレーキ装置を表す <see cref="ClassWrappers.BasicBrake"/> を取得します。
        /// </summary>
        public BasicBrake BasicBrake => ClassWrappers.BasicBrake.FromSource(BasicBrakeGetMethod.Invoke(Src, null));
    }
}
