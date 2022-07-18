using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.ExtendedBeacons
{
    /// <summary>
    /// 拡張地上子上を他列車が通過したときに発生するイベントのデータを提供します。
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

        public TrainPassedEventArgs(string senderTrainName, Train senderTrain, Direction direction) : base(direction)
        {
            SenderTrainName = senderTrainName;
            SenderTrain = senderTrain;
        }

        public TrainPassedEventArgs(TrainPassedEventArgs original, IReadOnlyDictionary<string, dynamic> scriptVariables) : base(original, scriptVariables)
        {
            SenderTrainName = original.SenderTrainName;
            SenderTrain = original.SenderTrain;
        }
    }
}
