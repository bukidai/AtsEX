using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.ExtendedBeacons;
using AtsEx.PluginHost.ClassWrappers;

namespace AtsEx.PluginHost.MapStatements
{
    /// <summary>
    /// ユーザー定義ステートメント上を他列車が通過したときに発生するイベントのデータを提供します。
    /// </summary>
    public class TrainPassedEventArgs : PassedEventArgs
    {
        /// <summary>
        /// 通過した他列車の名前を取得します。
        /// </summary>
        /// <remarks>名前は全て小文字に変換されていることに注意してください。</remarks>
        public string SenderTrainName { get; }

        /// <summary>
        /// 通過した他列車を表す <see cref="Train"/> を取得します。
        /// </summary>
        public Train SenderTrain { get; }

        /// <summary>
        /// <see cref="TrainPassedEventArgs"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="senderTrainName">通過した他列車の名前。名前は全て小文字としてください。</param>
        /// <param name="senderTrain">通過した他列車を表す <see cref="Train"/>。</param>
        /// <param name="direction">通過した列車の進行方向。</param>
        public TrainPassedEventArgs(string senderTrainName, Train senderTrain, Direction direction) : base(direction)
        {
            SenderTrainName = senderTrainName;
            SenderTrain = senderTrain;
        }
    }
}
