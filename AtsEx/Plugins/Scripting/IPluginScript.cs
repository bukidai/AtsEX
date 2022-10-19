using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.Plugins.Scripting
{
    internal interface IPluginScript<TGlobals> : ICloneable where TGlobals : Globals
    {
        IPluginScript<TGlobals> GetWithCheckErrors();
        Task<IScriptResult> RunAsync(TGlobals globals);
    }

    internal interface IPluginScript<TResult, TGlobals> : IPluginScript<TGlobals> where TGlobals : Globals
    {
        new IPluginScript<TResult, TGlobals> GetWithCheckErrors();
        new Task<IScriptResult<TResult>> RunAsync(TGlobals globals);
    }
}
