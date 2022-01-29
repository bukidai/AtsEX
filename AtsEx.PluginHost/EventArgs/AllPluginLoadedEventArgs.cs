using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost
{
    public class AllPluginLoadedEventArgs : EventArgs
    {
        public List<AtsExPluginInfo> Plugins { get; }

        public AllPluginLoadedEventArgs(List<AtsExPluginInfo> plugins) : base()
        {
            Plugins = plugins;
        }
    }
}
