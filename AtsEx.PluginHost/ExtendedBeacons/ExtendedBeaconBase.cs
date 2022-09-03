using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.ExtendedBeacons
{
    public delegate void PassedEventHandler<TPassedEventArgs>(object sender, TPassedEventArgs e) where TPassedEventArgs : PassedEventArgs;

    /// <summary>
    /// すべての拡張地上子の基本クラスを表します。
    /// </summary>
    /// <typeparam name="TPassedEventArgs"><see cref="Passed"/> イベントで使用する <see cref="EventArgs"/> の型。</typeparam>
    public abstract class ExtendedBeaconBase<TPassedEventArgs> : IExtendedBeacon where TPassedEventArgs : PassedEventArgs
    {
        /// <summary>
        /// 検知対象の列車がこの地上子上を通過した瞬間に発生します。
        /// </summary>
        public event PassedEventHandler<TPassedEventArgs> Passed;

        /// <summary>
        /// この地上子の名前を取得します。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// この地上子の設置されている距離程 [m] を取得します。
        /// </summary>
        public double Location => DefinedStructure.Location;

        /// <summary>
        /// この地上子の定義に使用されている連続ストラクチャーを取得します。
        /// </summary>
        public RepeatedStructure DefinedStructure { get; }

        /// <summary>
        /// この地上子が列車の通過を検知する対象の軌道を指定する <see cref="PluginHost.ExtendedBeacons.ObservingTargetTrack"/> を取得します。
        /// </summary>
        public ObservingTargetTrack ObservingTargetTrack { get; }

        /// <summary>
        /// この地上子が通過を検知する対象の列車を指定する <see cref="PluginHost.ExtendedBeacons.ObservingTargetTrain"/> を取得します。
        /// </summary>
        public ObservingTargetTrain ObservingTargetTrain { get; }

        /// <summary>
        /// <see cref="ExtendedBeaconBase{TPassedEventArgs}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">地上子の名前。</param>
        /// <param name="definedStructure">地上子の定義に使用する連続ストラクチャー。</param>
        /// <param name="observingTargetTrack">検知対象の軌道。</param>
        /// <param name="observingTargetTrain">検知対象の列車。</param>
        public ExtendedBeaconBase(string name, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, ObservingTargetTrain observingTargetTrain)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DefinedStructure = definedStructure ?? throw new ArgumentNullException(nameof(definedStructure));
            ObservingTargetTrack = observingTargetTrack;
            ObservingTargetTrain = observingTargetTrain;
        }

        /// <summary>
        /// 検知対象の列車が通過したことをこの <see cref="ExtendedBeaconBase{TPassedEventArgs}"/> オブジェクトに通知します。
        /// </summary>
        protected void NotifyPassed(TPassedEventArgs eventArgs)
        {
            Passed?.Invoke(this, eventArgs);
        }

        /// <inheritdoc/>
        public int CompareTo(IExtendedBeacon other) => (int)(Location - other.Location);
    }

    /// <summary>
    /// Repeater 構文によって定義される拡張地上子を表します。
    /// </summary>
    public interface IExtendedBeacon : IComparable<IExtendedBeacon>
    {
        /// <summary>
        /// 設置されている距離程 [m] を取得します。
        /// </summary>
        double Location { get; }

        /// <summary>
        /// この地上子の定義に使用された連続ストラクチャーを取得します。
        /// </summary>
        RepeatedStructure DefinedStructure { get; }
    }
}
