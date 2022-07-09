using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.Plugins.Scripting
{
    internal interface IPluginScript<TGlobals> where TGlobals : Globals
    {
        IScriptResult Run(TGlobals globals);
    }

    internal interface IPluginScript : IPluginScript<Globals>
    {
    }
}
