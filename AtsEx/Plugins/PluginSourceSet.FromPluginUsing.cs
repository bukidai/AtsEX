using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using AtsEx.Plugins.Scripting;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Plugins
{
    internal sealed partial class PluginSourceSet
    {
        public static PluginSourceSet FromPluginUsing(PluginType pluginType, bool allowNonPluginAssembly, string listPath)
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

            return new PluginSourceSet(Path.GetFileName(listPath), pluginType, allowNonPluginAssembly, assemblies, cSharpScriptPackages, ironPython2Packages);


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
