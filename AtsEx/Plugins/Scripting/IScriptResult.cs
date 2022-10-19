using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.Plugins.Scripting
{
    internal interface IScriptResult
    {
        T GetVariable<T>(string name);
    }

    internal interface IScriptResult<TResult> : IScriptResult
    {
        TResult ReturnValue { get; }
    }
}
