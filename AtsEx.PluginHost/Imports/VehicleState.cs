using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    public class VehicleState
    {
        public VehicleState(double location, float speed, TimeSpan time, float bcPressure, float mrPressure, float erPressure, float bpPressure, float sapPressure, float current)
        {
            Location = location;
            Speed = speed;
            Time = time;

            BcPressure = bcPressure;
            MrPressure = mrPressure;
            ErPressure = erPressure;
            BpPressure = bpPressure;
            SapPressure = sapPressure;
            Current = current;
        }

        public double Location { get; }
        public float Speed { get; }
        public TimeSpan Time { get; }

        public float BcPressure { get; }
        public float MrPressure { get; }
        public float ErPressure { get; }
        public float BpPressure { get; }
        public float SapPressure { get; }
        public float Current { get; }
    }
}
