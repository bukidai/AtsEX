﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx.Plugins.Scripting
{
    public class StartedGlobals : Globals
    {
#pragma warning disable IDE1006 // 命名スタイル
        public readonly StartedEventArgs e;
#pragma warning restore IDE1006 // 命名スタイル

        public StartedGlobals(Globals source, StartedEventArgs e) : base(source)
        {
            this.e = e;
        }
    }
}
