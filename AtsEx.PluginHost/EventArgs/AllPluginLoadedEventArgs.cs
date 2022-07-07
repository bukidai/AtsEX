using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost.Plugins
{
    public class AllPluginLoadedEventArgs : EventArgs
    {
        public List<PluginBase> Plugins { get; }

        public AllPluginLoadedEventArgs(List<PluginBase> plugins) : base()
        {
            Plugins = plugins;
        }
    }
}
