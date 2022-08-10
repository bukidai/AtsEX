using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.PluginHost.ExtendedBeacons
{
    /// <summary>
    /// 拡張地上子の一覧を表します。
    /// </summary>
    public abstract class ExtendedBeaconSet
    {
        /// <summary>
        /// 自列車の通過を検知する地上子の一覧を取得します。
        /// </summary>
        public abstract ReadOnlyDictionary<string, ExtendedBeaconBase<PassedEventArgs>> Beacons { get; }

        /// <summary>
        /// 他列車の通過を検知する地上子の一覧を取得します。
        /// </summary>
        public abstract ReadOnlyDictionary<string, ExtendedBeaconBase<TrainPassedEventArgs>> TrainObservingBeacons { get; }

        /// <summary>
        /// 先行列車の通過を検知する地上子の一覧を取得します。
        /// </summary>
        public abstract ReadOnlyDictionary<string, ExtendedBeaconBase<PassedEventArgs>> PreTrainObservingBeacons { get; }

        internal protected abstract T GetPluginVariable<T>(PluginBase target, string name);
    }
}
