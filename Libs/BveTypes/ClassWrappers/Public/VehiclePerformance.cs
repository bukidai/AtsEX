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
    /// 自列車の車両性能を表します。
    /// </summary>
    public class VehiclePerformance : VehiclePerformanceBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<VehiclePerformance>();
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="VehiclePerformance"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected VehiclePerformance(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="VehiclePerformance"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static new VehiclePerformance FromSource(object src) => src is null ? null : new VehiclePerformance(src);
    }
}
