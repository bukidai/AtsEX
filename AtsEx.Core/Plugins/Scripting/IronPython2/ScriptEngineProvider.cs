using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using IronPython.Hosting;

using Microsoft.Scripting.Hosting;

namespace Automatic9045.AtsEx.Plugins.Scripting.IronPython2
{
    internal static class ScriptEngineProvider
    {
        public static readonly ScriptEngine ScriptEngine;

        private static readonly IEnumerable<string> PluginHostNamespaces;

        static ScriptEngineProvider()
        {
            ScriptEngine = Python.CreateEngine();

            ScriptEngine.Runtime.LoadAssembly(typeof(Form).Assembly);
            ScriptEngine.Runtime.LoadAssembly(App.Instance.AtsExPluginHostAssembly);

            PluginHostNamespaces = App.Instance.AtsExPluginHostAssembly.GetTypes().Select(t => t.Namespace).Distinct().Where(n => !(n is null));
        }
    }
}
