using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using UnembeddedResources;

using Automatic9045.AtsEx.Plugins.Scripting;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.Plugins
{
    internal class PluginUsing
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<PluginUsing>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> BadImageFormat { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> XmlSchemaValidation { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        protected static XmlSchemaSet SchemaSet = new XmlSchemaSet();
        protected static string TargetNamespace;

        public string Name { get; }

        public PluginType PluginType { get; }

        protected readonly SortedList<Identifier, Assembly> _Assemblies;
        public ReadOnlyDictionary<Identifier, Assembly> Assemblies { get; }

        protected readonly SortedList<Identifier, ScriptPluginPackage> _CSharpScriptPackages;
        public ReadOnlyDictionary<Identifier, ScriptPluginPackage> CSharpScriptPackages { get; }

        protected readonly SortedList<Identifier, ScriptPluginPackage> _IronPython2Packages;
        public ReadOnlyDictionary<Identifier, ScriptPluginPackage> IronPython2Packages { get; }

        static PluginUsing()
        {
#if DEBUG
            _ = Resources.Value;
#endif

            using (Stream schemaStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(PluginUsing).Namespace}.AtsExPluginUsingXmlSchema.xsd"))
            {
                XmlSchema schema = XmlSchema.Read(schemaStream, SchemaValidation);
                TargetNamespace = $"{{{schema.TargetNamespace}}}";
                SchemaSet.Add(schema);
            }
        }

        protected PluginUsing(string name, PluginType pluginType,
            IDictionary<Identifier, Assembly> assemblies, IDictionary<Identifier, ScriptPluginPackage> csharpScriptPackages, IDictionary<Identifier, ScriptPluginPackage> ironPython2Packages)
        {
            Name = name;
            PluginType = pluginType;

            _Assemblies = new SortedList<Identifier, Assembly>(assemblies);
            _CSharpScriptPackages = new SortedList<Identifier, ScriptPluginPackage>(csharpScriptPackages);
            _IronPython2Packages = new SortedList<Identifier, ScriptPluginPackage>(ironPython2Packages);

            Assemblies = new ReadOnlyDictionary<Identifier, Assembly>(_Assemblies);
            CSharpScriptPackages = new ReadOnlyDictionary<Identifier, ScriptPluginPackage>(_CSharpScriptPackages);
            IronPython2Packages = new ReadOnlyDictionary<Identifier, ScriptPluginPackage>(_IronPython2Packages);
        }

        public static PluginUsing Load(PluginType pluginType, string listPath)
        {
            XDocument doc = XDocument.Load(listPath, LoadOptions.SetLineInfo);
            doc.Validate(SchemaSet, DocumentValidation);

            XElement root = doc.Element(TargetNamespace + "AtsExPluginUsing");

            Dictionary<Identifier, Assembly> assemblies = root.Elements(TargetNamespace + "Assembly").
                ToDictionary(GetIdentifier, element => LoadAssembly(element, listPath));

            Dictionary<Identifier, ScriptPluginPackage> cSharpScriptPackages = root.Elements(TargetNamespace + "CSharpScript").
                ToDictionary(GetIdentifier, element => LoadScriptPluginPackage(element, Path.GetDirectoryName(listPath)));

            Dictionary<Identifier, ScriptPluginPackage> ironPython2Packages = root.Elements(TargetNamespace + "IronPython2").
                ToDictionary(GetIdentifier, element => LoadScriptPluginPackage(element, Path.GetDirectoryName(listPath)));

            return new PluginUsing(Path.GetFileName(listPath), pluginType, assemblies, cSharpScriptPackages, ironPython2Packages);


            Identifier GetIdentifier(XElement element)
            {
                string identifierText = (string)element.Attribute("Identifier");
                return identifierText is null ? new RandomIdentifier() : new Identifier(identifierText);
            }
        }

        private static Assembly LoadAssembly(XElement element, string listPath)
        {
            IXmlLineInfo lineInfo = element;

            string assemblyPath = element.Attribute("Path").Value;
            try
            {
                return Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(listPath), assemblyPath));
            }
            catch (BadImageFormatException)
            {
                int currentBveVersion = App.Instance.BveAssembly.GetName().Version.Major;
                int otherBveVersion = currentBveVersion == 6 ? 5 : 6;
                throw new BveFileLoadException(
                    string.Format(Resources.Value.BadImageFormat.Value, Path.GetDirectoryName(assemblyPath), otherBveVersion, App.Instance.ProductShortName, currentBveVersion),
                    Path.GetFileName(listPath), lineInfo.LineNumber, lineInfo.LinePosition);;
            }
        }

        private static ScriptPluginPackage LoadScriptPluginPackage(XElement element, string baseDirectory)
        {
            string packageManifestPath = element.Attribute("PackageManifestPath").Value;
            return ScriptPluginPackage.Load(Path.Combine(baseDirectory, packageManifestPath));
        }

        private static void SchemaValidation(object sender, ValidationEventArgs e) => throw new FormatException(Resources.Value.XmlSchemaValidation.Value, e.Exception);

        private static void DocumentValidation(object sender, ValidationEventArgs e) => throw e.Exception;
    }
}
