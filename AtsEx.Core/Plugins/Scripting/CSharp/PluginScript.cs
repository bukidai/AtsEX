using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Automatic9045.AtsEx.Plugins.Scripting.CSharp
{
    internal class PluginScript<TGlobals> : IPluginScript<TGlobals> where TGlobals : Globals
    {
        protected static readonly ScriptOptions ScriptOptions = ScriptOptions.Default.
            AddReferences(typeof(System.Windows.Forms.Form).Assembly).
            WithImports("System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Windows.Forms").
            AddReferences(App.Instance.AtsExPluginHostAssembly).
            AddImports(App.Instance.AtsExPluginHostAssembly.GetTypes().Select(t => t.Namespace).Distinct().Where(n => !(n is null)));

        protected readonly Script Script;

        public PluginScript(string code)
        {
            Script = CSharpScript.Create(code, ScriptOptions, typeof(TGlobals));
            Compile();
        }

        public PluginScript(Stream code)
        {
            Script = CSharpScript.Create(code, ScriptOptions, typeof(TGlobals));
            Compile();
        }

        public static PluginScript<TGlobals> LoadFrom(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return new PluginScript<TGlobals>(stream);
            }
        }

        private void Compile()
        {
            ImmutableArray<Diagnostic> compilationErrors = Script.Compile();
            if (compilationErrors.Any()) throw new CompilationException(compilationErrors);
        }

        public IScriptResult Run(TGlobals globals)
        {
            ScriptState state = Script.RunAsync(globals).Result;
            return new ScriptResult(state);
        }
    }
}
