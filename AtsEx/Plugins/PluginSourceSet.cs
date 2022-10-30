using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

using UnembeddedResources;

using AtsEx.Plugins.Scripting;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Plugins
{
    internal sealed partial class PluginSourceSet
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<PluginSourceSet>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> BadImageFormat { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> XmlSchemaValidation { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        private static readonly XmlSchemaSet SchemaSet = new XmlSchemaSet();
        private static readonly string TargetNamespace;

        static PluginSourceSet()
        {
#if DEBUG
            _ = Resources.Value;
#endif

            using (Stream schemaStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(PluginSourceSet).Namespace}.AtsExPluginUsingXmlSchema.xsd"))
            {
                XmlSchema schema = XmlSchema.Read(schemaStream, SchemaValidation);
                TargetNamespace = $"{{{schema.TargetNamespace}}}";
                SchemaSet.Add(schema);
            }
        }

        public string Name { get; }

        public PluginType PluginType { get; }

        public ReadOnlyDictionary<Identifier, Assembly> Assemblies { get; }
        public ReadOnlyDictionary<Identifier, ScriptPluginPackage> CSharpScriptPackages { get; }
        public ReadOnlyDictionary<Identifier, ScriptPluginPackage> IronPython2Packages { get; }

        private PluginSourceSet(string name, PluginType pluginType,
            IDictionary<Identifier, Assembly> assemblies, IDictionary<Identifier, ScriptPluginPackage> csharpScriptPackages, IDictionary<Identifier, ScriptPluginPackage> ironPython2Packages)
        {
            Name = name;
            PluginType = pluginType;

            Assemblies = new ReadOnlyDictionary<Identifier, Assembly>(assemblies);
            CSharpScriptPackages = new ReadOnlyDictionary<Identifier, ScriptPluginPackage>(csharpScriptPackages);
            IronPython2Packages = new ReadOnlyDictionary<Identifier, ScriptPluginPackage>(ironPython2Packages);
        }

        public static PluginSourceSet Empty(PluginType pluginType)
            => new PluginSourceSet(null, pluginType, new Dictionary<Identifier, Assembly>(), new Dictionary<Identifier, ScriptPluginPackage>(), new Dictionary<Identifier, ScriptPluginPackage>());
    }
}
