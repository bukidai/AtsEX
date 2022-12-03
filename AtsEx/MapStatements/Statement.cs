using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost;
using AtsEx.PluginHost.MapStatements;

namespace AtsEx.MapStatements
{
    internal sealed class Statement : IStatement
    {
        private readonly IEnumerable<TracableTrain> TargetTrains;

        private double OldVehicleLocation = 0d;
        private double OldPreTrainLocation = 0;

        public Identifier Name { get; }
        public Identifier[] AdditionalDeclaration { get; }
        public string Argument { get; }

        public double From => DefinedStructure.Location;
        public double To { get; }
        public RepeatedStructure DefinedStructure { get; }

        public Statement(Identifier name, Identifier[] additionalDeclaration, string argument, RepeatedStructure definedStructure, double to, IEnumerable<TracableTrain> targetTrains)
        {
            Name = name;
            AdditionalDeclaration = additionalDeclaration;
            Argument = argument;

            DefinedStructure = definedStructure;
            To = to;

            TargetTrains = targetTrains;
        }

        public void Tick(double vehicleLocation, double preTrainLocation)
        {
            {
                if (OldVehicleLocation < From && From <= vehicleLocation)
                {
                    Entered?.Invoke(this, new PassedEventArgs(Direction.Forward));
                }
                else if (To < OldVehicleLocation && vehicleLocation <= To)
                {
                    Entered?.Invoke(this, new PassedEventArgs(Direction.Backward));
                }

                if (OldVehicleLocation < To && To <= vehicleLocation)
                {
                    Exited?.Invoke(this, new PassedEventArgs(Direction.Forward));
                }
                else if (From < OldVehicleLocation && vehicleLocation <= From)
                {
                    Exited?.Invoke(this, new PassedEventArgs(Direction.Backward));
                }


                OldVehicleLocation = vehicleLocation;
            }

            {
                if (OldPreTrainLocation < From && From <= preTrainLocation)
                {
                    PreTrainEntered?.Invoke(this, new PassedEventArgs(Direction.Forward));
                }
                else if (To < OldPreTrainLocation && preTrainLocation <= To)
                {
                    PreTrainEntered?.Invoke(this, new PassedEventArgs(Direction.Backward));
                }

                if (OldPreTrainLocation < To && To <= preTrainLocation)
                {
                    PreTrainExited?.Invoke(this, new PassedEventArgs(Direction.Forward));
                }
                else if (From < OldPreTrainLocation && preTrainLocation <= From)
                {
                    PreTrainExited?.Invoke(this, new PassedEventArgs(Direction.Backward));
                }

                OldPreTrainLocation = preTrainLocation;
            }

            foreach (TracableTrain train in TargetTrains)
            {
                train.UpdateLocation();

                if (train.OldLocation < From && From <= train.Location)
                {
                    TrainEntered?.Invoke(this, new TrainPassedEventArgs(train.Name, train.Train, Direction.Forward));
                }
                else if (From < train.OldLocation && train.Location <= From)
                {
                    TrainEntered?.Invoke(this, new TrainPassedEventArgs(train.Name, train.Train, Direction.Backward));
                }

                if (train.OldLocation < To && To <= train.Location)
                {
                    TrainExited?.Invoke(this, new TrainPassedEventArgs(train.Name, train.Train, Direction.Forward));
                }
                else if (From < train.OldLocation && train.Location <= From)
                {
                    TrainExited?.Invoke(this, new TrainPassedEventArgs(train.Name, train.Train, Direction.Backward));
                }
            }
        }

        public event EventHandler<PassedEventArgs> Entered;
        public event EventHandler<PassedEventArgs> PreTrainEntered;
        public event EventHandler<TrainPassedEventArgs> TrainEntered;

        public event EventHandler<PassedEventArgs> Exited;
        public event EventHandler<PassedEventArgs> PreTrainExited;
        public event EventHandler<TrainPassedEventArgs> TrainExited;

        internal class TracableTrain
        {
            public string Name { get; }
            public Train Train { get; }

            public double OldLocation { get; private set; }
            public double Location { get; private set; }

            public TracableTrain(string name, Train train)
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
