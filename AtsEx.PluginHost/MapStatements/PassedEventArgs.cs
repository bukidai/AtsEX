using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.ExtendedBeacons;

namespace AtsEx.PluginHost.MapStatements
{
    /// <summary>
    /// ユーザー定義ステートメント上を列車が通過したときに発生するイベントのデータを提供します。
    /// </summary>
    public class PassedEventArgs : EventArgs
    {
        /// <summary>
        /// 通過した列車の進行方向を取得します。
        /// </summary>
        public Direction Direction { get; }

        /// <summary>
        /// <see cref="PassedEventArgs"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="direction">通過した列車の進行方向。</param>
        public PassedEventArgs(Direction direction)
        {
            Direction = direction;
        }
    }
}
