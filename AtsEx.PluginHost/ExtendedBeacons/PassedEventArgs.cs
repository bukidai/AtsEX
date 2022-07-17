using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ExtendedBeacons
{
    public class PassedEventArgs : EventArgs
    {
        public Direction Direction { get; }

        public PassedEventArgs(Direction direction)
        {
            Direction = direction;
        }
    }
}
