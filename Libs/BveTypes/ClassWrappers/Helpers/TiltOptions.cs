using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// マップ オブジェクトを傾斜させる方法を指定します。
    /// </summary>
    [Flags]
    public enum TiltOptions
    {
        /// <summary>
        /// 傾斜させないことを指定します。
        /// </summary>
        Default = 0,

        /// <summary>
        /// 勾配に連動して傾斜させることを指定します。
        /// </summary>
        TiltsAlongGradient = 1,

        /// <summary>
        /// カントに連動して傾斜させることを指定します。
        /// </summary>
        TiltsAlongCant = 2,
    }
}
