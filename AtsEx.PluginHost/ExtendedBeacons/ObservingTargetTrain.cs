using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.ExtendedBeacons
{
    /// <summary>
    /// 地上子が列車を検知する条件を指定します。
    /// </summary>
    public enum ObservingTargetTrain
    {
        /// <summary>
        /// 自列車の通過を検知することを指定します。
        /// </summary>
        Myself,

        /// <summary>
        /// 他列車の通過を検知することを指定します。
        /// </summary>
        Trains,

        /// <summary>
        /// 先行列車の通過を検知することを指定します。
        /// </summary>
        PreTrain,
    }
}
