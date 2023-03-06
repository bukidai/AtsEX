using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// 客室ドアの開閉状態を指定します。
    /// </summary>
    public enum DoorState
    {
        /// <summary>
        /// ドアが閉まっていること、あるいはドアを閉めることを指定します。
        /// </summary>
        Close,
        /// <summary>
        /// ドアが開いていること、あるいはドアを開けることを指定します。
        /// </summary>
        Open,
    }
}
