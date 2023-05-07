using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Plugins.Scripting;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Plugins
{
    internal sealed partial class PluginSourceSet
    {
        public static PluginSourceSet FromDirectory(string name, PluginType pluginType, bool allowNonPluginAssembly, string directoryName)
        {
            Dictionary<Identifier, Assembly> assemblies =
                Directory.GetFiles(directoryName, "*.dll", SearchOption.AllDirectories)
                .ToDictionary(x => (Identifier)new RandomIdentifier(), Assembly.LoadFrom);

            return new PluginSourceSet(name, pluginType, allowNonPluginAssembly, assemblies, new Dictionary<Identifier, ScriptPluginPackage>(), new Dictionary<Identifier, ScriptPluginPackage>());
        }
    }
}
