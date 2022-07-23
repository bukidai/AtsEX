using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Handles;

namespace Automatic9045.AtsEx.Handles
{
    internal class Reverser : IReverser
    {
        public ReverserPosition Position { get; set; } = ReverserPosition.N;

        public Reverser()
        {
        }
    }
}
