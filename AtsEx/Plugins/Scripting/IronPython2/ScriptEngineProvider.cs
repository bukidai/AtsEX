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

using AtsEx.PluginHost;

namespace AtsEx.Plugins.Scripting.IronPython2
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

            ImportAssembliesFromArray("System", "System.Collections.Generic", "System.Text", "System.Windows.Forms");
            ImportAssemblies(App.Instance.AtsExPluginHostAssembly.GetTypes().Where(t => t.IsPublic).Select(t => t.Namespace).Distinct().Where(n => !(n is null)));

            ICollection<string> engineSearchPaths = scriptEngine.GetSearchPaths();
            foreach (string path in searchPaths) engineSearchPaths.Add(path);
            scriptEngine.SetSearchPaths(engineSearchPaths);

            return scriptEngine;


            void ImportAssembly(string name) => scriptEngine.Execute($"import {name}");

            void ImportAssemblies(IEnumerable<string> names)
            {
                foreach (string name in names)
                {
                    ImportAssembly(name);
                }
            }

            void ImportAssembliesFromArray(params string[] names) => ImportAssemblies(names);
        }

        public static ScriptEngine CreateEngine(params string[] searchPaths) => CreateEngine(searchPaths as ICollection<string>);

        public static ScriptScope CreateScope(ScriptEngine engine)
        {
            ScriptScope scope = engine.CreateScope();
            return scope;
        }
    }
}
