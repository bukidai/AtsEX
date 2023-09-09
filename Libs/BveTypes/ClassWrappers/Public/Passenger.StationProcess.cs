using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BveTypes.ClassWrappers
{
    public partial class Passenger
    {
        /// <summary>
        /// 停車場における自列車の乗客の乗降の進捗を指定します。
        /// </summary>
        public enum StationProcess
        {
            /// <summary>
            /// 乗降が開始されていないことを指定します。
            /// </summary>
            Ready,

            /// <summary>
            /// 降車中であることを指定します。
            /// </summary>
            Alighting,

            /// <summary>
            /// 乗車中であることを指定します。
            /// </summary>
            Boarding,

            /// <summary>
            /// 乗降が完了したことを指定します。
            /// </summary>
            Completed,
        }
    }
}
