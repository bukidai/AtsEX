using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.ClassWrappers;

namespace AtsEx.PluginHost.Native
{
    public class StartedEventArgs : EventArgs
    {
        public BrakePosition DefaultBrakePosition { get; }

        public StartedEventArgs(BrakePosition defaultBrakePosition) : base()
        {
            DefaultBrakePosition = defaultBrakePosition;
        }
    }
}
