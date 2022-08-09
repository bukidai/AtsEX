using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Scripting.Hosting;

namespace Automatic9045.AtsEx.Plugins.Scripting.IronPython2
{
    internal partial class PluginScript<TGlobals> : IPluginScript<TGlobals> where TGlobals : Globals
    {
        public string Name { get; } = null;

        private Task<CompiledCode> CompilationTask;
        protected readonly ScriptScope Scope;
        protected readonly ScriptSource Source;
        protected CompiledCode CompiledCode;

        protected PluginScript(ScriptSource source, ScriptScope scope, string name)
        {
            Source = source;
            Scope = scope;
            Name = name;

            BeginCompile();
        }

        public PluginScript(string code, ScriptEngine engine, ScriptScope scope, string name) : this(engine.CreateScriptSourceFromString(code), scope, name)
        {
        }

        public static PluginScript<TGlobals> LoadFrom(string path, ScriptEngine engine, ScriptScope scope)
        {
            ScriptSource source = LoadSource(path, engine);
            return new PluginScript<TGlobals>(source, scope, Path.GetFileName(path));
        }

        protected static ScriptSource LoadSource(string path, ScriptEngine engine) => engine.CreateScriptSourceFromFile(path, Encoding.UTF8);

        private void BeginCompile()
        {
            CompilationTask = Task.Run(() =>
            {
                CompiledCode compiledCode = Source.Compile();
                return compiledCode;
            });
        }

        public IPluginScript<TGlobals> GetWithCheckErrors()
        {
            try
            {
                CompiledCode = CompilationTask.Result;
                return this;
            }
            catch (AggregateException ex)
            {
                throw ex.GetBaseException();
            }
        }

        public IScriptResult Run(TGlobals globals)
        {
            ExecuteCode(globals);
            return new ScriptResult(Scope);
        }

        protected void ExecuteCode(TGlobals globals)
        {
            if (!CompilationTask.IsCompleted) GetWithCheckErrors();

            Scope.SetVariable("g", globals);
            CompiledCode.Execute(Scope);
        }
    }

    internal class PluginScript<TResult, TGlobals> : PluginScript<TGlobals>, IPluginScript<TResult, TGlobals> where TGlobals : Globals
    {
        protected PluginScript(ScriptSource source, ScriptScope scope, string name) : base(source, scope, name)
        {
        }

        public PluginScript(string code, ScriptEngine engine, ScriptScope scope, string name) : base(engine.CreateScriptSourceFromString(code), scope, name)
        {
        }

        public static new PluginScript<TResult, TGlobals> LoadFrom(string path, ScriptEngine engine, ScriptScope scope)
        {
            ScriptSource source = LoadSource(path, engine);
            return new PluginScript<TResult, TGlobals>(source, scope, Path.GetFileName(path));
        }

        public new IPluginScript<TResult, TGlobals> GetWithCheckErrors() => base.GetWithCheckErrors() as PluginScript<TResult, TGlobals>;

        public new IScriptResult<TResult> Run(TGlobals globals)
        {
            ExecuteCode(globals);
            return new ScriptResult<TResult>(Scope);
        }
    }
}
