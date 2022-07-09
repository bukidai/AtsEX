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

using Automatic9045.AtsEx.Plugins.Scripting;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.Plugins
{
    internal class PluginUsing
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(PluginUsing), "Core");

        protected static XmlSchemaSet SchemaSet = new XmlSchemaSet();
        protected static string TargetNamespace;

        public PluginType PluginType { get; }

        protected readonly List<Assembly> _Assemblies;
        public ReadOnlyCollection<Assembly> Assemblies { get; }

        protected readonly List<ScriptPluginPackage> _CSharpScripts;
        public ReadOnlyCollection<ScriptPluginPackage> CSharpScripts { get; }

        static PluginUsing()
        {
            using (Stream schemaStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(PluginUsing).Namespace}.AtsExPluginUsingXmlSchema.xsd"))
            {
                XmlSchema schema = XmlSchema.Read(schemaStream, SchemaValidation);
                TargetNamespace = $"{{{schema.TargetNamespace}}}";
                SchemaSet.Add(schema);
            }
        }

        protected PluginUsing(PluginType pluginType, IEnumerable<Assembly> assemblies, IEnumerable<ScriptPluginPackage> csharpScripts)
        {
            PluginType = pluginType;

            _Assemblies = assemblies.ToList();
            _CSharpScripts = csharpScripts.ToList();

            Assemblies = _Assemblies.AsReadOnly();
            CSharpScripts = _CSharpScripts.AsReadOnly();
        }

        public static PluginUsing Load(PluginType pluginType, string listPath)
        {
            XDocument doc = XDocument.Load(listPath, LoadOptions.SetLineInfo);
            doc.Validate(SchemaSet, DocumentValidation);

            XElement root = doc.Element(TargetNamespace + "AtsExPluginUsing");

            IEnumerable<Assembly> assemblies = root.Elements(TargetNamespace + "Assembly").Select(element => LoadAssembly(element, listPath));
            IEnumerable<ScriptPluginPackage> cSharpScriptPluginPackages = root.Elements(TargetNamespace + "CSharpScript").Select(element => LoadScriptPluginPackage(element, Path.GetDirectoryName(listPath)));

            return new PluginUsing(pluginType, assemblies, cSharpScriptPluginPackages);
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
                    string.Format(Resources.GetString("BadImageFormat").Value, Path.GetDirectoryName(assemblyPath), otherBveVersion, App.Instance.ProductShortName, currentBveVersion),
                    Path.GetFileName(listPath), lineInfo.LineNumber, lineInfo.LinePosition);;
            }
        }

        private static ScriptPluginPackage LoadScriptPluginPackage(XElement element, string baseDirectory)
        {
            string packageManifestPath = element.Attribute("PackageManifestPath").Value;
            return ScriptPluginPackage.Load(Path.Combine(baseDirectory, packageManifestPath));
        }

        private static void SchemaValidation(object sender, ValidationEventArgs e) => throw new FormatException(Resources.GetString("XmlSchemaValidation").Value, e.Exception);

        private static void DocumentValidation(object sender, ValidationEventArgs e) => throw e.Exception;
    }
}
