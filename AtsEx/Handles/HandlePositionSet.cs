using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost.Handles;

namespace AtsEx.Handles
{
    internal class HandlePositionSet
    {
        public int Power { get; }
        public int Brake { get; }
        public ReverserPosition ReverserPosition { get; }
        public ConstantSpeedCommand ConstantSpeed { get; }

        public HandlePositionSet(int power, int brake, ReverserPosition reverserPosition, ConstantSpeedCommand constantSpeedCommand)
        {
            Power = power;
            Brake = brake;
            ReverserPosition = reverserPosition;
            ConstantSpeed = constantSpeedCommand;
        }
    }
}
