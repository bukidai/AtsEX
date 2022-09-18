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

namespace Automatic9045.AtsEx.Plugins.Scripting
{
    internal partial class ScriptPluginPackage
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(ScriptPluginPackage), "Core");

        protected static XmlSchemaSet SchemaSet = new XmlSchemaSet();
        protected static string TargetNamespace;

        public bool UseAtsExExtensions { get; }

        public string Location { get; }
        public string Title { get; }
        public string Version { get; }
        public string Description { get; }
        public string Copyright { get; }

        public string ConstructorScriptPath { get; }
        public string DisposeScriptPath { get; }
        public string OnScenarioCreatedScriptPath { get; }
        public string OnStartedScriptPath { get; }
        public string TickScriptPath { get; }

        static ScriptPluginPackage()
        {
            using (Stream schemaStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(ScriptPluginPackage).Namespace}.AtsExScriptPluginPackageManifestXmlSchema.xsd"))
            {
                XmlSchema schema = XmlSchema.Read(schemaStream, SchemaValidation);
                TargetNamespace = $"{{{schema.TargetNamespace}}}";
                SchemaSet.Add(schema);
            }
        }

        protected ScriptPluginPackage(InformationBuilder informationBuilder, ScriptsBuilder scriptsBuilder, bool useAtsExExtensions)
        {
            UseAtsExExtensions = useAtsExExtensions;

            Location = informationBuilder.Location;
            Title = informationBuilder.Title;
            Version = informationBuilder.Version;
            Description = informationBuilder.Description;
            Copyright = informationBuilder.Copyright;

            ConstructorScriptPath = scriptsBuilder.ConstructorScriptPath;
            DisposeScriptPath = scriptsBuilder.DisposeScriptPath;
            OnScenarioCreatedScriptPath = scriptsBuilder.OnScenarioCreatedScriptPath;
            OnStartedScriptPath = scriptsBuilder.OnStartedScriptPath;
            TickScriptPath = scriptsBuilder.TickScriptPath;
        }

        public static ScriptPluginPackage Load(string path)
        {
            XDocument doc = XDocument.Load(path, LoadOptions.SetLineInfo);
            doc.Validate(SchemaSet, DocumentValidation);

            XElement root = doc.Element(TargetNamespace + "AtsExScriptPluginPackageManifest");

            XElement infoElement = root.Element(TargetNamespace + "Info");
            InformationBuilder informationBuilder = CreateInformationBuilder(infoElement, path);

            XElement scriptsElement = root.Element(TargetNamespace + "Scripts");
            bool useAtsExExtensions = (bool?)scriptsElement.Attribute("UseAtsExExtensions") ?? true;
            ScriptsBuilder scriptsBuilder = CreateScriptsBuilder(scriptsElement, Path.GetDirectoryName(path));

            return new ScriptPluginPackage(informationBuilder, scriptsBuilder, useAtsExExtensions);
        }

        private static InformationBuilder CreateInformationBuilder(XElement infoElement, string path)
        {
            string title = GetElementValue("Title");
            InformationBuilder builder = new InformationBuilder(path, title)
            {
                Version = GetElementValue("Version"),
                Description = GetElementValue("Description"),
                Copyright = GetElementValue("Copyright"),
            };

            return builder;


            string GetElementValue(string name) => (string)infoElement.Element(TargetNamespace + name);
        }

        private static ScriptsBuilder CreateScriptsBuilder(XElement scriptsElement, string baseDirectory)
        {
            ScriptsBuilder builder = new ScriptsBuilder
            {
                ConstructorScriptPath = GetElementPath("Constructor"),
                DisposeScriptPath = GetElementPath("Dispose"),
                OnScenarioCreatedScriptPath = GetElementPath("OnScenarioCreated"),
                OnStartedScriptPath = GetElementPath("OnStarted"),
                TickScriptPath = GetElementPath("Tick"),
            };

            return builder;

            string GetElementPath(string name)
            {
                string path = (string)scriptsElement.Element(TargetNamespace + name)?.Attribute("Path");
                return path is null ? null : Path.Combine(baseDirectory, path);
            }
        }

        private static void SchemaValidation(object sender, ValidationEventArgs e) => throw new FormatException(Resources.GetString("XmlSchemaValidation").Value, e.Exception);

        private static void DocumentValidation(object sender, ValidationEventArgs e) => throw e.Exception;
    }
}
