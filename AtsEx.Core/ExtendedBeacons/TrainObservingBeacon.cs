using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.Plugins.Scripting;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal sealed class TrainObservingBeacon : ExtendedBeaconBase<TrainPassedEventArgs>
    {
        private readonly List<TrainInfo> TargetTrains;

        public TrainObservingBeacon(BveHacker bveHacker, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, IDictionary<string, Train> targetTrains, IPluginScript<ExtendedBeaconGlobalsBase<TrainPassedEventArgs>> script)
            : base(bveHacker, definedStructure, observingTargetTrack, script)
        {
            TargetTrains = targetTrains.Select(train => new TrainInfo(train.Key, train.Value)).ToList();
        }

        internal void Tick()
        {
            foreach (TrainInfo train in TargetTrains)
            {
                train.UpdateLocation();

                if (train.OldLocation < Location && Location <= train.Location)
                {
                    NotifyPassed(train, Direction.Forward);
                }
                else if (Location < train.OldLocation && train.Location <= Location)
                {
                    NotifyPassed(train, Direction.Backward);
                }
            }


            void NotifyPassed(TrainInfo senderTrain, Direction direction)
            {
                TrainPassedEventArgs eventArgs = new TrainPassedEventArgs(senderTrain.Name, senderTrain.Train, direction);
                Script.Run(new TrainPassedGlobals(BveHacker, this, eventArgs));
                base.NotifyPassed(eventArgs);
            }
        }


        class TrainInfo
        {
            public string Name { get; }
            public Train Train { get; }

            public double OldLocation { get; private set; }
            public double Location { get; private set; }

            public TrainInfo(string name, Train train)
            {
                Name = name;
                Train = train;

                OldLocation = train.Location;
                Location = train.Location;
            }

            public void UpdateLocation()
            {
                OldLocation = Location;
                Location = Train.Location;
            }
        }
    }
}
