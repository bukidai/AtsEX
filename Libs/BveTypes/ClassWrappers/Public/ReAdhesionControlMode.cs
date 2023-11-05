using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// 再粘着制御の動作状態を指定します。
    /// </summary>
    public enum ReAdhesionControlMode
    {
        /// <summary>
        /// 再粘着制御を動作させないことを指定します。
        /// </summary>
        Disable,

        /// <summary>
        /// 電流または圧力を引き下げることを指定します。
        /// </summary>
        Reduce,

        /// <summary>
        /// 低い電流または圧力のまま維持することを指定します。
        /// </summary>
        Hold,

        /// <summary>
        /// 引き下げた電流または圧力を元に戻すことを指定します。
        /// </summary>
        Return,
    }
}
