using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Scripting.Hosting;

namespace Automatic9045.AtsEx.Plugins.Scripting.IronPython2
{
    internal class ScriptResult : IScriptResult
    {
        protected ScriptScope Scope;

        public ScriptResult(ScriptScope scope)
        {
            Scope = scope;
        }

        public T GetVariable<T>(string name) => Scope.GetVariable<T>(name);
    }

    internal class ScriptResult<TResult> : ScriptResult, IScriptResult<TResult>
    {
        public TResult ReturnValue { get; }

        public ScriptResult(TResult returnValue, ScriptScope scope) : base(scope)
        {
            ReturnValue = returnValue;
        }
    }
}
