using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    public class VehicleSpec
    {
        public VehicleSpec(int brakeNotches, int powerNotches, int atsNotch, int b67notch, int cars)
        {
            BrakeNotches = brakeNotches;
            PowerNotches = powerNotches;
            AtsNotch = atsNotch;
            B67Notch = b67notch;
            Cars = cars;
        }

        public int BrakeNotches { get; }
        public int PowerNotches { get; }
        public int AtsNotch { get; }
        public int B67Notch { get; }
        public int Cars { get; }
    }
}
