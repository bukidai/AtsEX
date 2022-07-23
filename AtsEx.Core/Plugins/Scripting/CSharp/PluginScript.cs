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

        protected PluginScript(Script script, string name)
        {
            Script = script;
            Name = name;
            BeginCompile();
        }

        public PluginScript(string code, ScriptSourceResolver scriptSourceResolver, string name)
            : this(CSharpScript.Create(code, ScriptOptions.WithSourceResolver(scriptSourceResolver), typeof(TGlobals)), name)
        {
        }

        public PluginScript(string code, string searchBaseDirectory, string name)
            : this(code, ScriptSourceResolver.Default.WithBaseDirectory(searchBaseDirectory), name)
        {
        }

        public PluginScript(string code, string name) : this(code, ScriptSourceResolver.Default, name)
        {
        }

        protected PluginScript(Stream code, ScriptOptions scriptOptions, string name) : this(CSharpScript.Create(code, scriptOptions, typeof(TGlobals)), name)
        {
        }

        public static PluginScript<TGlobals> LoadFrom(string path)
        {
            (ScriptOptions scriptOptions, string name) = LoadObjectsToConstruct(path);

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return new PluginScript<TGlobals>(stream, scriptOptions, name);
            }
        }

        protected static (ScriptOptions scriptOptions, string name) LoadObjectsToConstruct(string path)
        {
            ScriptSourceResolver scriptSourceResolver = ScriptSourceResolver.Default.WithBaseDirectory(Path.GetDirectoryName(path));
            ScriptOptions scriptOptions = ScriptOptions.WithSourceResolver(scriptSourceResolver);

            return (scriptOptions, Path.GetFileName(path));
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
            ScriptState state = ExecuteCode(globals);
            return new ScriptResult(state);
        }

        protected ScriptState ExecuteCode(TGlobals globals)
        {
            if (!CompilationTask.IsCompleted) GetWithCheckErrors();

            ScriptState state = Script.RunAsync(globals).Result;
            return state;
        }
    }

    internal class PluginScript<TResult, TGlobals> : PluginScript<TGlobals>, IPluginScript<TResult, TGlobals> where TGlobals : Globals
    {
        protected PluginScript(Script script, string name) : base(script, name)
        {
        }

        public PluginScript(string code, ScriptSourceResolver scriptSourceResolver, string name) : base(code, scriptSourceResolver, name)
        {
        }

        public PluginScript(string code, string searchBaseDirectory, string name) : base(code, searchBaseDirectory, name)
        {
        }

        public PluginScript(string code, string name) : base(code, name)
        {
        }

        protected PluginScript(Stream code, ScriptOptions scriptOptions, string name) : base(code, scriptOptions, name)
        {
        }

        public static new PluginScript<TResult, TGlobals> LoadFrom(string path)
        {
            (ScriptOptions scriptOptions, string name) = LoadObjectsToConstruct(path);

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return new PluginScript<TResult, TGlobals>(stream, scriptOptions, name);
            }
        }

        public new IPluginScript<TResult, TGlobals> GetWithCheckErrors() => base.GetWithCheckErrors() as PluginScript<TResult, TGlobals>;

        public new IScriptResult<TResult> Run(TGlobals globals)
        {
            ScriptState state = ExecuteCode(globals);
            return new ScriptResult<TResult>(state);
        }
    }
}
