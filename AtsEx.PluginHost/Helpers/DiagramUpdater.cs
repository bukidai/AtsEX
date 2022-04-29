using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.Helpers
{
    public static class DiagramUpdater
    {
        public static void Update()
        {
            InstanceStore.BveHacker.TimePosForm.SetScenario(InstanceStore.BveHacker.ScenarioProvider);
        }
    }
}
