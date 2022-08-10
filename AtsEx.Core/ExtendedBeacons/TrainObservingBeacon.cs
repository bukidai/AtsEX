using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.Plugins.Scripting;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal sealed class TrainObservingBeacon : ExtendedBeaconBase<TrainPassedEventArgs>
    {
        private readonly List<TrainInfo> TargetTrains;

        public TrainObservingBeacon(BveHacker bveHacker, IReadOnlyDictionary<PluginType, PluginVariableCollection> pluginVariables,
            string name, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, IDictionary<string, Train> targetTrains,
            IPluginScript<ExtendedBeaconGlobalsBase<TrainPassedEventArgs>> script)
            : base(bveHacker, pluginVariables, name, definedStructure, observingTargetTrack, ObservingTargetTrain.Trains, script)
        {
            TargetTrains = targetTrains.Select(train => new TrainInfo(train.Key, train.Value)).ToList();
        }

        internal void Tick()
        {
            foreach (TrainInfo train in TargetTrains)
            {
                train.UpdateLocation();

                if (train.OldLocation < Location && Location <= train.Location && IsCurrentTrackTarget(train.Train))
                {
                    NotifyPassed(train, Direction.Forward);
                }
                else if (Location < train.OldLocation && train.Location <= Location && IsCurrentTrackTarget(train.Train))
                {
                    NotifyPassed(train, Direction.Backward);
                }
            }


            bool IsCurrentTrackTarget(Train train) => ObservingTargetTrack == ObservingTargetTrack.AllTracks || train.TrainInfo.TrackKey == DefinedStructure.TrackKey;

            void NotifyPassed(TrainInfo senderTrain, Direction direction)
            {
                TrainPassedEventArgs eventArgs = new TrainPassedEventArgs(senderTrain.Name, senderTrain.Train, direction);
                TrainPassedGlobals globals = new TrainPassedGlobals(BveHacker, PluginVariables, this, eventArgs);
                Script.Run(globals);
                base.NotifyPassed(globals.GetEventArgsWithScriptVariables());
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
