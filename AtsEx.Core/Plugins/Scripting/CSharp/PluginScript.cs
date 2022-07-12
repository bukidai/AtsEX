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

        public string Name { get; } = null;

        private Task<ImmutableArray<Diagnostic>> CompilationTask;
        protected readonly Script Script;

        public PluginScript(string code, string name)
        {
            Script = CSharpScript.Create(code, ScriptOptions, typeof(TGlobals));
            Name = name;
            BeginCompile();
        }

        public PluginScript(Stream code, string name)
        {
            Script = CSharpScript.Create(code, ScriptOptions, typeof(TGlobals));
            Name = name;
            BeginCompile();
        }

        public static PluginScript<TGlobals> LoadFrom(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return new PluginScript<TGlobals>(stream, Path.GetFileName(path));
            }
        }

        private void BeginCompile()
        {
            CompilationTask = Task.Run(() =>
            {
                ImmutableArray<Diagnostic> compilationErrors = Script.Compile();
                return compilationErrors;
            });
        }

        public IPluginScript<TGlobals> GetWithCheckErrors()
        {
            ImmutableArray<Diagnostic> compilationErrors = CompilationTask.Result;
            return compilationErrors.Any() ? throw new CompilationException(Name, compilationErrors) : this;
        }

        public IScriptResult Run(TGlobals globals)
        {
            if (!CompilationTask.IsCompleted) GetWithCheckErrors();

            ScriptState state = Script.RunAsync(globals).Result;
            return new ScriptResult(state);
        }
    }
}
