using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    public class ScenarioProvider : ClassWrapper, IScenarioProvider
    {
        public ScenarioProvider(object src) : base(src)
        {
        }
    }
}
