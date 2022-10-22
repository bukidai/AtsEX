using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// ノッチの情報を表します。
    /// </summary>
    public class NotchInfo : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<NotchInfo>();

            PowerNotchCountGetMethod = members.GetSourcePropertyGetterOf(nameof(PowerNotchCount));
            PowerNotchCountSetMethod = members.GetSourcePropertySetterOf(nameof(PowerNotchCount));

            HoldingSpeedNotchCountGetMethod = members.GetSourcePropertyGetterOf(nameof(HoldingSpeedNotchCount));
            HoldingSpeedNotchCountSetMethod = members.GetSourcePropertySetterOf(nameof(HoldingSpeedNotchCount));

            BrakeNotchCountGetMethod = members.GetSourcePropertyGetterOf(nameof(BrakeNotchCount));
            BrakeNotchCountSetMethod = members.GetSourcePropertySetterOf(nameof(BrakeNotchCount));

            B67NotchGetMethod = members.GetSourcePropertyGetterOf(nameof(B67Notch));
            B67NotchSetMethod = members.GetSourcePropertySetterOf(nameof(B67Notch));

            AtsCancelNotchGetMethod = members.GetSourcePropertyGetterOf(nameof(AtsCancelNotch));
            AtsCancelNotchSetMethod = members.GetSourcePropertySetterOf(nameof(AtsCancelNotch));

            MotorBrakeNotchGetMethod = members.GetSourcePropertyGetterOf(nameof(MotorBrakeNotch));
            MotorBrakeNotchSetMethod = members.GetSourcePropertySetterOf(nameof(MotorBrakeNotch));

            HasHoldingSpeedBrakeGetMethod = members.GetSourcePropertyGetterOf(nameof(HasHoldingSpeedBrake));
            HasHoldingSpeedBrakeSetMethod = members.GetSourcePropertySetterOf(nameof(HasHoldingSpeedBrake));

            EmergencyBrakeNotchGetMethod = members.GetSourcePropertyGetterOf(nameof(EmergencyBrakeNotch));

            PrioritizeBrakeGetMethod = members.GetSourcePropertyGetterOf(nameof(PrioritizeBrake));
            PrioritizeBrakeSetMethod = members.GetSourcePropertySetterOf(nameof(PrioritizeBrake));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="NotchInfo"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected NotchInfo(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="NotchInfo"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static NotchInfo FromSource(object src) => src is null ? null : new NotchInfo(src);

        private static FastMethod PowerNotchCountGetMethod;
        private static FastMethod PowerNotchCountSetMethod;
        /// <summary>
        /// 力行ノッチ数を取得します。
        /// </summary>
        public int PowerNotchCount
        {
            get => PowerNotchCountGetMethod.Invoke(Src, null);
            internal set => PowerNotchCountSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod HoldingSpeedNotchCountGetMethod;
        private static FastMethod HoldingSpeedNotchCountSetMethod;
        /// <summary>
        /// 抑速ノッチ数を取得します。
        /// </summary>
        /// <remarks>
        /// 値は負で指定されます。
        /// </remarks>
        public int HoldingSpeedNotchCount
        {
            get => HoldingSpeedNotchCountGetMethod.Invoke(Src, null);
            internal set => HoldingSpeedNotchCountSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod BrakeNotchCountGetMethod;
        private static FastMethod BrakeNotchCountSetMethod;
        /// <summary>
        /// ブレーキノッチ数を取得します。
        /// </summary>
        public int BrakeNotchCount
        {
            get => BrakeNotchCountGetMethod.Invoke(Src, null);
            internal set => BrakeNotchCountSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod B67NotchGetMethod;
        private static FastMethod B67NotchSetMethod;
        /// <summary>
        /// ブレーキ弁 67 度に相当するノッチを取得します。
        /// </summary>
        public int B67Notch
        {
            get => B67NotchGetMethod.Invoke(Src, null);
            internal set => B67NotchSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod AtsCancelNotchGetMethod;
        private static FastMethod AtsCancelNotchSetMethod;
        /// <summary>
        /// ATS 確認扱いで必要な最小ブレーキノッチを取得します。
        /// </summary>
        public int AtsCancelNotch
        {
            get => AtsCancelNotchGetMethod.Invoke(Src, null);
            internal set => AtsCancelNotchSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod MotorBrakeNotchGetMethod;
        private static FastMethod MotorBrakeNotchSetMethod;
        /// <summary>
        /// 電気ブレーキが立ち上がる最小ブレーキノッチを取得します。
        /// </summary>
        public int MotorBrakeNotch
        {
            get => MotorBrakeNotchGetMethod.Invoke(Src, null);
            internal set => MotorBrakeNotchSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod HasHoldingSpeedBrakeGetMethod;
        private static FastMethod HasHoldingSpeedBrakeSetMethod;
        /// <summary>
        /// ブレーキノッチ 1 段目が抑速ブレーキかどうかを取得します。
        /// </summary>
        public bool HasHoldingSpeedBrake
        {
            get => HasHoldingSpeedBrakeGetMethod.Invoke(Src, null);
            internal set => HasHoldingSpeedBrakeSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod EmergencyBrakeNotchGetMethod;
        /// <summary>
        /// 電気ブレーキが立ち上がる最小ブレーキノッチを取得します。
        /// </summary>
        public int EmergencyBrakeNotch => EmergencyBrakeNotchGetMethod.Invoke(Src, null);

        private static FastMethod PrioritizeBrakeGetMethod;
        private static FastMethod PrioritizeBrakeSetMethod;
        /// <summary>
        /// 力行指令よりも電気ブレーキ指令を優先するかどうかを取得します。
        /// </summary>
        public bool PrioritizeBrake
        {
            get => PrioritizeBrakeGetMethod.Invoke(Src, null);
            internal set => PrioritizeBrakeSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
