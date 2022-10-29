using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Plugins.Scripting;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Plugins
{
    internal interface IPluginSourceSet
    {
        string Name { get; }

        PluginType PluginType { get; }

        ReadOnlyDictionary<Identifier, Assembly> Assemblies { get; }
        ReadOnlyDictionary<Identifier, ScriptPluginPackage> CSharpScriptPackages { get; }
        ReadOnlyDictionary<Identifier, ScriptPluginPackage> IronPython2Packages { get; }
    }
}
