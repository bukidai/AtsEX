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
    public class ScenarioProviderCreatedEventArgs : EventArgs
    {
        public ScenarioProvider ScenarioProvider { get; }

        public ScenarioProviderCreatedEventArgs(ScenarioProvider scenarioProvider) : base()
        {
            ScenarioProvider = scenarioProvider;
        }
    }
}
