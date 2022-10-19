﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AtsEx.PluginHost.ClassWrappers;

namespace AtsEx.PluginHost
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
