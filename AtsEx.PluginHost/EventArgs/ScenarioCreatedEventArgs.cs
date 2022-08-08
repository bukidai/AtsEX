using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost
{
    public class ScenarioCreatedEventArgs : EventArgs
    {
        public Scenario Scenario { get; }

        internal ScenarioCreatedEventArgs(Scenario scenario) : base()
        {
            Scenario = scenario;
        }
    }
}
