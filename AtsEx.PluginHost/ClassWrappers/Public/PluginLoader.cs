using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mackoy.Bvets;

using FastMember;
using TypeWrapping;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// プラグインの読込機能を提供します。
    /// </summary>
    public class PluginLoader : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<PluginLoader>();

            OnSetBeaconDataMethod = members.GetSourceMethodOf(nameof(OnSetBeaconData));
            OnKeyDownMethod = members.GetSourceMethodOf(nameof(OnKeyDown));
            OnKeyUpMethod = members.GetSourceMethodOf(nameof(OnKeyUp));
            OnDoorStateChangedMethod = members.GetSourceMethodOf(nameof(OnDoorStateChanged));
            OnSetSignalMethod = members.GetSourceMethodOf(nameof(OnSetSignal));
            OnSetReverserMethod = members.GetSourceMethodOf(nameof(OnSetReverser));
            OnSetBrakeMethod = members.GetSourceMethodOf(nameof(OnSetBrake));
            OnSetPowerMethod = members.GetSourceMethodOf(nameof(OnSetPower));
            OnSetVehicleSpecMethod = members.GetSourceMethodOf(nameof(OnSetVehicleSpec));
            OnInitializeMethod = members.GetSourceMethodOf(nameof(OnInitialize));
            OnElapseMethod = members.GetSourceMethodOf(nameof(OnElapse));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="PluginLoader"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected PluginLoader(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="PluginLoader"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static PluginLoader FromSource(object src) => src is null ? null : new PluginLoader(src);

        private static FastMethod OnSetBeaconDataMethod;
        /// <summary>
        /// プラグインに地上子を越えたことを通知します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データを格納しているオブジェクト。</param>
        public void OnSetBeaconData(object sender, ObjectPassedEventArgs e) => OnSetBeaconDataMethod.Invoke(this, new object[] { sender, e.Src });

        private static FastMethod OnKeyDownMethod;
        /// <summary>
        /// プラグインに ATS キー、または警笛キーが押されたことを通知します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データを格納しているオブジェクト。</param>
        public void OnKeyDown(object sender, InputEventArgs e) => OnKeyDownMethod.Invoke(this, new object[] { sender, e });

        private static FastMethod OnKeyUpMethod;
        /// <summary>
        /// プラグインに ATS キーが離されたことを通知します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データを格納しているオブジェクト。</param>
        public void OnKeyUp(object sender, InputEventArgs e) => OnKeyUpMethod.Invoke(this, new object[] { sender, e });

        private static FastMethod OnDoorStateChangedMethod;
        /// <summary>
        /// プラグインに客室ドアの状態が変化したことを通知します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データを格納しているオブジェクト。</param>
        public void OnDoorStateChanged(object sender, EventArgs e) => OnDoorStateChangedMethod.Invoke(this, new object[] { sender, e });

        private static FastMethod OnSetSignalMethod;
        /// <summary>
        /// プラグインに現在の閉塞の信号が変化したことを通知します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データを格納しているオブジェクト。</param>
        public void OnSetSignal(object sender, EventArgs e) => OnSetSignalMethod.Invoke(this, new object[] { sender, e });

        private static FastMethod OnSetReverserMethod;
        /// <summary>
        /// プラグインにレバーサーが扱われたことを通知します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データを格納しているオブジェクト。</param>
        public void OnSetReverser(object sender, EventArgs e) => OnSetReverserMethod.Invoke(this, new object[] { sender, e });

        private static FastMethod OnSetBrakeMethod;
        /// <summary>
        /// プラグインにブレーキが扱われたことを通知します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データを格納しているオブジェクト。</param>
        public void OnSetBrake(object sender, ValueEventArgs<int> e) => OnSetBrakeMethod.Invoke(this, new object[] { sender, e.Src });

        private static FastMethod OnSetPowerMethod;
        /// <summary>
        /// プラグインに主ハンドルが扱われたことを通知します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データを格納しているオブジェクト。</param>
        public void OnSetPower(object sender, ValueEventArgs<int> e) => OnSetPowerMethod.Invoke(this, new object[] { sender, e.Src });

        private static FastMethod OnSetVehicleSpecMethod;
        /// <summary>
        /// プラグインに車両スペックを設定します。
        /// </summary>
        /// <param name="notchInfo">ノッチの情報。</param>
        /// <param name="carCount">編成両数。</param>
        public void OnSetVehicleSpec(NotchInfo notchInfo, int carCount) => OnSetVehicleSpecMethod.Invoke(this, new object[] { notchInfo.Src, carCount });

        private static FastMethod OnInitializeMethod;
        /// <summary>
        /// プラグインにゲームが開始されたことを通知します。
        /// </summary>
        /// <param name="brakePosition">ゲーム開始時のブレーキ弁の状態。</param>
        public void OnInitialize(BrakePosition brakePosition) => OnInitializeMethod.Invoke(this, new object[] { brakePosition });

        private static FastMethod OnElapseMethod;
        /// <summary>
        /// プラグインに 1 フレームが経過したことを通知します。
        /// </summary>
        /// <param name="time">現在時刻。</param>
        public void OnElapse(int time) => OnElapseMethod.Invoke(this, new object[] { time });
    }
}
