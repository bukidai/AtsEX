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
    /// 自列車の揺れを制御します。
    /// </summary>
    public class VehicleVibrationManager : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<VehicleVibrationManager>();

            PositionerGetMethod = members.GetSourcePropertyGetterOf(nameof(Positioner));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="VehicleVibrationManager"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected VehicleVibrationManager(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="VehicleVibrationManager"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static VehicleVibrationManager FromSource(object src) => src is null ? null : new VehicleVibrationManager(src);

        private static FastMethod PositionerGetMethod;
        /// <summary>
        /// 自列車をマップ上に配置するための機能を提供する <see cref="VehiclePositioner"/> を取得します。
        /// </summary>
        public VehiclePositioner Positioner => VehiclePositioner.FromSource(PositionerGetMethod.Invoke(Src, null));
    }
}
