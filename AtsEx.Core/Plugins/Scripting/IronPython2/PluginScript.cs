using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IronPython.Hosting;

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

        public PluginScript(string code, ScriptScope scope, string name) : this(ScriptEngineProvider.ScriptEngine.CreateScriptSourceFromString(code), scope, name)
        {
        }

        public static PluginScript<TGlobals> LoadFrom(string path, ScriptScope scope)
        {
            ScriptSource source = ScriptEngineProvider.ScriptEngine.CreateScriptSourceFromFile(path, Encoding.UTF8);
            return new PluginScript<TGlobals>(source, scope, Path.GetFileName(path));
        }

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
            if (!CompilationTask.IsCompleted) GetWithCheckErrors();

            Scope.SetVariable("g", globals);
            CompiledCode.Execute(Scope);
            return new ScriptResult();
        }
    }
}
