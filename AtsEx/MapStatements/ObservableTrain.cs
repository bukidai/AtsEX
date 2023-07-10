using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

namespace AtsEx.MapStatements
{
    internal class ObservableTrain
    {
        public string Name { get; }
        public Train Train { get; }

        public double OldLocation { get; private set; }
        public double Location { get; private set; }

        public ObservableTrain(string name, Train train)
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
