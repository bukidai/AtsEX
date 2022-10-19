using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AtsEx.PluginHost.Plugins
{
    public class AllPluginLoadedEventArgs : EventArgs
    {
        public ReadOnlyDictionary<string, PluginBase> Plugins { get; }

        public AllPluginLoadedEventArgs(ReadOnlyDictionary<string, PluginBase> plugins) : base()
        {
            Plugins = plugins;
        }
    }
}
