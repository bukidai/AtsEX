using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.ExtendedBeacons
{
    public class TrainPassedEventArgs : PassedEventArgs
    {
        public string SenderTrainName { get; }
        public Train SenderTrain { get; }

        public TrainPassedEventArgs(string senderTrainName, Train senderTrain, Direction direction) : base(direction)
        {
            SenderTrainName = senderTrainName;
            SenderTrain = senderTrain;
        }
    }
}
