using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

using UnembeddedResources;

namespace AtsEx.Plugins
{
    internal class VehicleConfig
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<VehicleConfig>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> XmlSchemaValidation { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        private static readonly XmlSchemaSet SchemaSet = new XmlSchemaSet();
        private static readonly string TargetNamespace;

        public static readonly VehicleConfig Default = new VehicleConfig();

        static VehicleConfig()
        {
#if DEBUG
            _ = Resources.Value;
#endif

            using (Stream schemaStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(VehicleConfig).Namespace}.AtsExVehicleConfigXmlSchema.xsd"))
            {
                XmlSchema schema = XmlSchema.Read(schemaStream, SchemaValidation);
                TargetNamespace = $"{{{schema.TargetNamespace}}}";
                SchemaSet.Add(schema);
            }
        }

        public bool DetectSoundIndexConflict { get; private set; } = false;
        public bool DetectPanelValueIndexConflict { get; private set; } = false;

        private VehicleConfig()
        {
        }

        public static VehicleConfig LoadFrom(string path)
        {
            XDocument doc = XDocument.Load(path, LoadOptions.SetLineInfo);
            doc.Validate(SchemaSet, DocumentValidation);

            XElement root = doc.Element(TargetNamespace + "AtsExVehicleConfig");

            VehicleConfig result = new VehicleConfig()
            {
                DetectSoundIndexConflict = (bool?)root.Element(TargetNamespace + "DetectSoundIndexConflict") ?? false,
                DetectPanelValueIndexConflict = (bool?)root.Element(TargetNamespace + "DetectPanelValueIndexConflict") ?? false,
            };

            return result;
        }

        private static void SchemaValidation(object sender, ValidationEventArgs e) => throw new FormatException(Resources.Value.XmlSchemaValidation.Value, e.Exception);

        private static void DocumentValidation(object sender, ValidationEventArgs e) => throw e.Exception;
    }
}
