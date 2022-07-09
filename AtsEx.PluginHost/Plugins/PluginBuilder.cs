﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Plugins
{
    public class PluginBuilder
    {
        public IApp App { get; }
        public IBveHacker BveHacker { get; private set; } = null;

        public PluginBuilder(IApp app)
        {
            App = app;
        }

        public PluginBuilder UseAtsExExtensions(IBveHacker bveHacker)
        {
            BveHacker = bveHacker;
            return this;
        }
    }
}
