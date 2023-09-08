using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BveTypes.ClassWrappers
{
    public partial class Sound
    {
        /// <summary>
        /// サウンドの設置位置を指定します。
        /// </summary>
        public enum SoundPosition
        {
            /// <summary>
            /// サウンドを地上に設置することを指定します。
            /// </summary>
            Ground,

            /// <summary>
            /// サウンドを自列車の運転台上に設置することを指定します。
            /// </summary>
            Cab,
        }
    }
}
