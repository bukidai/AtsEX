using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BveTypes.ClassWrappers
{
    public partial class TimeManager
    {
        /// <summary>
        /// ゲームの時間進行の設定を指定します。
        /// </summary>
        public enum GameState
        {
            /// <summary>
            /// 通常。
            /// </summary>
            Forward,

            /// <summary>
            /// ポーズ中。
            /// </summary>
            Paused,

            /// <summary>
            /// 早送り。
            /// </summary>
            FastForward,
        }
    }
}
