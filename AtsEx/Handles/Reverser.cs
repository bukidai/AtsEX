using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.Handles;

namespace AtsEx.Handles
{
    internal class Reverser : IReverser
    {
        public ReverserPosition Position { get; set; } = ReverserPosition.N;

        public Reverser()
        {
        }
    }
}
