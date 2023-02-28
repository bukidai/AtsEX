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
        private double OldVehicleLocation = double.NegativeInfinity;
        private double OldPreTrainLocation = double.NegativeInfinity;

        public Identifier Name { get; }
        public Identifier[] AdditionalDeclaration { get; }
        public string Argument { get; }

        public double From => DefinedStructure.Location;
        public double To { get; }
        public RepeatedStructure DefinedStructure { get; }

        public Statement(Identifier name, Identifier[] additionalDeclaration, string argument, RepeatedStructure definedStructure, double to)
        {
            Name = name;
            AdditionalDeclaration = additionalDeclaration;
            Argument = argument;

            DefinedStructure = definedStructure;
            To = to;
        }

        public void Tick(double vehicleLocation, double preTrainLocation, IEnumerable<MonitorableTrain> trainsToMonitor)
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

            foreach (MonitorableTrain train in trainsToMonitor)
            {
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
    }
}
