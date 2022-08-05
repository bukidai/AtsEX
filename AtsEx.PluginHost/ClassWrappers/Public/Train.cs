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
    /// 他列車を表します。
    /// </summary>
    public class Train : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<Train>();

            TrainInfoField = members.GetSourceFieldOf(nameof(TrainInfo));
            LocationField = members.GetSourceFieldOf(nameof(Location));
            SpeedField = members.GetSourceFieldOf(nameof(Speed));
        }

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

        private static FieldInfo TrainInfoField;
        /// <summary>
        /// この他列車の情報を提供する <see cref="ClassWrappers.TrainInfo"/> を取得・設定します。
        /// </summary>
        public TrainInfo TrainInfo
        {
            get => ClassWrappers.TrainInfo.FromSource(TrainInfoField.GetValue(Src));
            set => TrainInfoField.SetValue(Src, value.Src);
        }

        private static FieldInfo LocationField;
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

        private static FieldInfo SpeedField;
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
    }
}
