using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SlimDX;

using FastMember;
using TypeWrapping;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 他列車を表します。
    /// </summary>
    public class Train : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<Train>();

            UserVehicleLocationManagerField = members.GetSourceFieldOf(nameof(UserVehicleLocationManager));
            RouteField = members.GetSourceFieldOf(nameof(Route));
            TrainInfoField = members.GetSourceFieldOf(nameof(TrainInfo));
            DrawDistanceManagerField = members.GetSourceFieldOf(nameof(DrawDistanceManager));
            LocationField = members.GetSourceFieldOf(nameof(Location));
            SpeedField = members.GetSourceFieldOf(nameof(Speed));

            DrawCarsMethod = members.GetSourceMethodOf(nameof(DrawCars));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="Train"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected Train(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Train"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static Train FromSource(object src) => src is null ? null : new Train(src);

        private static FastField UserVehicleLocationManagerField;
        internal UserVehicleLocationManager UserVehicleLocationManager => ClassWrappers.UserVehicleLocationManager.FromSource(UserVehicleLocationManagerField.GetValue(Src));

        private static FastField RouteField;
        internal Route Route => ClassWrappers.Route.FromSource(RouteField.GetValue(Src));

        private static FastField TrainInfoField;
        /// <summary>
        /// この他列車の情報を提供する <see cref="ClassWrappers.TrainInfo"/> を取得・設定します。
        /// </summary>
        public TrainInfo TrainInfo
        {
            get => ClassWrappers.TrainInfo.FromSource(TrainInfoField.GetValue(Src));
            set => TrainInfoField.SetValue(Src, value.Src);
        }

        private static FastField DrawDistanceManagerField;
        internal DrawDistanceManager DrawDistanceManager => ClassWrappers.DrawDistanceManager.FromSource(DrawDistanceManagerField.GetValue(Src));

        private static FastField LocationField;
        /// <summary>
        /// この他列車の現在位置 [m] を取得・設定します。
        /// </summary>
        /// <remarks>
        /// 距離程が正の方向を正とします。
        /// </remarks>
        public double Location
        {
            get => (double)LocationField.GetValue(Src);
            set => LocationField.SetValue(Src, value);
        }

        private static FastField SpeedField;
        /// <summary>
        /// この他列車の速度 [m/s] を取得・設定します。
        /// </summary>
        /// <remarks>
        /// 距離程が正の方向を正とします。<br/>
        /// この値は走行音のピッチの設定にのみ使用されます。車両ストラクチャーのアニメーションには <see cref="Location"/> プロパティを使用してください。
        /// </remarks>
        public double Speed
        {
            get => (double)SpeedField.GetValue(Src);
            set => SpeedField.SetValue(Src, value);
        }

        private static FastMethod DrawCarsMethod;
        /// <summary>
        /// この他列車を構成する車両オブジェクトを描画します。
        /// </summary>
        /// <param name="direct3DProvider">描画に使用する <see cref="Direct3DProvider"/>。</param>
        /// <param name="additionalWorldMatrix">ワールド変換行列の後に追加で掛ける行列。</param>
        public void DrawCars(Direct3DProvider direct3DProvider, Matrix additionalWorldMatrix)
            => DrawCarsMethod.Invoke(Src, new object[] { direct3DProvider.Src, additionalWorldMatrix });
    }
}
