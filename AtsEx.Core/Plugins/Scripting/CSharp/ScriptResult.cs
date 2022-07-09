using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Automatic9045.AtsEx.Plugins.Scripting.CSharp
{
    internal class ScriptResult : IScriptResult
    {
        protected readonly ScriptState State;

        public ScriptResult(ScriptState state)
        {
            State = state;
        }
    }
}
