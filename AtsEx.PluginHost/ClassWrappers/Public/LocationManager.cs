using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 車両の位置に関する処理を行います。
    /// </summary>
    public class LocationManager : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<LocationManager>();

            SpeedMeterPerSecondGetMethod = members.GetSourcePropertyGetterOf(nameof(SpeedMeterPerSecond));

            SetSpeedMethod = members.GetSourceMethodOf(nameof(SetSpeed));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="LocationManager"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected LocationManager(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="LocationManager"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static LocationManager FromSource(object src) => src is null ? null : new LocationManager(src);

        private static FastMethod SpeedMeterPerSecondGetMethod;
        /// <summary>
        /// 車両の速度 [m/s] を取得します。
        /// </summary>
        /// <remarks>
        /// 車両の速度の変更には <see cref="SetSpeed(double)"/> メソッドを使用してください。
        /// </remarks>
        /// <seealso cref="SetSpeed(double)"/>
        public double SpeedMeterPerSecond => SpeedMeterPerSecondGetMethod.Invoke(Src, null);

        private static FastMethod SetSpeedMethod;
        /// <summary>
        /// 車両の速度を設定します。
        /// </summary>
        /// <param name="speedMeterPerSecond">車両の速度 [m/s]。</param>
        /// <remarks>
        /// 車両の速度の取得には <see cref="SpeedMeterPerSecond"/> プロパティを使用してください。
        /// </remarks>
        /// <seealso cref="SpeedMeterPerSecond"/>
        public void SetSpeed(double speedMeterPerSecond) => SetSpeedMethod.Invoke(Src, new object[] { speedMeterPerSecond });
    }
}
