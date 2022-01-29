using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost
{
    public interface IVehicle
    {
        int BrakeNotches { get; }
        int PowerNotches { get; }
        int AtsNotch { get; }
        int B67Notch { get; }
        int Cars { get; }

        double LocationD { get; }
        int Location { get; set; }

        int Speed { get; set; }
        float DisplaySpeed { get; }

        float BcPressure { get; }
        float MrPressure { get; }
        float ErPressure { get; }
        float BpPressure { get; }
        float SapPressure { get; }
        float Current { get; }

        int BrakeNotch { get; set; }
        int PowerNotch { get; set; }
        ReverserPosition ReverserPosition { get; set; }
        ConstantSpeedState ConstantSpeedState { get; set; }
    }
}
