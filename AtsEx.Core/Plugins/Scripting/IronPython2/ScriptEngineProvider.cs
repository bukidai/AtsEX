using System;
using System.Collections.Generic;
using System.IO;
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
        private static readonly IEnumerable<string> PluginHostNamespaces;

        static ScriptEngineProvider()
        {
            PluginHostNamespaces = App.Instance.AtsExPluginHostAssembly.GetTypes().Select(t => t.Namespace).Distinct().Where(n => !(n is null));
        }

        public static ScriptEngine CreateEngine(ICollection<string> searchPaths)
        {
            ScriptEngine scriptEngine = Python.CreateEngine();

            scriptEngine.Runtime.LoadAssembly(typeof(Form).Assembly);
            scriptEngine.Runtime.LoadAssembly(App.Instance.AtsExPluginHostAssembly);

            ICollection<string> engineSearchPaths = scriptEngine.GetSearchPaths();
            foreach (string path in searchPaths) engineSearchPaths.Add(path);
            scriptEngine.SetSearchPaths(engineSearchPaths);

            return scriptEngine;
        }

        public static ScriptEngine CreateEngine(params string[] searchPaths) => CreateEngine(searchPaths as ICollection<string>);

        public static ScriptScope CreateScope(ScriptEngine engine)
        {
            ScriptScope scope = engine.CreateScope();
            return scope;
        }
    }
}
