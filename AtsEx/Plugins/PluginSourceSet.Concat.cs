using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Plugins.Scripting;

namespace AtsEx.Plugins
{
    internal sealed partial class PluginSourceSet
    {
        public PluginSourceSet Concat(string name, PluginSourceSet other)
        {
            if (PluginType != other.PluginType) throw new InvalidOperationException();

            IDictionary<Identifier, Assembly> concatenatedAssemblies = ConcatDictionaries(Assemblies, other.Assemblies);
            IDictionary<Identifier, ScriptPluginPackage> concatenatedCSharpScriptPackages = ConcatDictionaries(CSharpScriptPackages, other.CSharpScriptPackages);
            IDictionary<Identifier, ScriptPluginPackage> concatenatedIronPython2Packages = ConcatDictionaries(IronPython2Packages, other.IronPython2Packages);

            return new PluginSourceSet(name, PluginType, concatenatedAssemblies, concatenatedCSharpScriptPackages, concatenatedIronPython2Packages);

            Dictionary<TKey, TValue> ConcatDictionaries<TKey, TValue>(IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
                => Enumerable.Concat(first, second).GroupBy(pair => pair.Value).Select(x => x.First()).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
