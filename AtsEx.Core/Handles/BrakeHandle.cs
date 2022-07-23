using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.Handles
{
    internal class BrakeHandle : HandleBase, IBrakeHandle
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<BrakeHandle>(@"Core\Handles");
        
        public bool HasHoldingSpeedBrake { get; }
        public int HoldingSpeedBrakeNotch => HasHoldingSpeedBrake ? 1 : throw new InvalidOperationException(Resources.GetString("NoHoldingSpeedBrake").Value);
        public int ServiceBrakeNotchCount => MaxNotch - 1 - (HasHoldingSpeedBrake ? 1 : 0);
        public int MinServiceBrakeNotch => HasHoldingSpeedBrake ? 2 : 1;
        public int MaxServiceBrakeNotch => MaxNotch - 1;
        public int EmergencyBrakeNotch => MaxNotch;

        /// <summary>
        /// <see cref="BrakeHandle"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="serviceBrakeNotchCount">抑速ブレーキノッチを除いた常用ブレーキの段数。</param>
        /// <param name="hasHoldingSpeedBrake">ブレーキハンドルに抑速ブレーキノッチがあるかどうか。</param>
        public BrakeHandle(int serviceBrakeNotchCount, bool hasHoldingSpeedBrake) : base(0, (hasHoldingSpeedBrake ? 1 : 0) + serviceBrakeNotchCount + 1)
        {
            if (serviceBrakeNotchCount <= 0) throw new ArgumentOutOfRangeException(nameof(serviceBrakeNotchCount));

            HasHoldingSpeedBrake = hasHoldingSpeedBrake;
        }

        public static BrakeHandle FromNotchInfo(NotchInfo source)
        {
            int serviceBrakeNotchCount = source.BrakeNotchCount;
            bool hasHoldingSpeedBrake = source.HasHoldingSpeedBrake;

            return new BrakeHandle(serviceBrakeNotchCount - (hasHoldingSpeedBrake ? 1 : 0), hasHoldingSpeedBrake);
        }

        public NotchCommandBase GetCommandToSetToNeutral() => new NotchCommandBase.SetNotchCommand(0);
    }
}
