using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

namespace AtsEx.MapPlugins.ExtendedBeacons
{
    /// <summary>
    /// 拡張地上子が検知する対象の軌道を指定します。
    /// </summary>
    public enum ObservingTargetTrack
    {
        /// <summary>
        /// <see cref="RepeatedStructure"/> が設置されている軌道上を走行する列車のみを検知します。
        /// </summary>
        SpecifiedTrackOnly,

        /// <summary>
        /// 全ての軌道上を走行する列車を検知します。
        /// </summary>
        AllTracks,
    }
}
