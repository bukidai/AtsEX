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

namespace TypeWrapping
{
    public static partial class WrapTypesXmlLoader
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType(typeof(WrapTypesXmlLoader), @"TypeWrapping\WrapTypesXmlLoader");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> XmlSchemaValidation { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly ResourceSet Resources = new ResourceSet();

        public static List<TypeMemberSetBase> LoadFile(Stream docStream, Stream schemaStream,
            IEnumerable<Type> wrapperTypes, IEnumerable<Type> originalTypes, IDictionary<Type, Type> additionalWrapperToOriginal)
        {
            XDocument doc = XDocument.Load(docStream);

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            XmlSchema schema = XmlSchema.Read(schemaStream, SchemaValidation);
            schemaSet.Add(schema);
            doc.Validate(schemaSet, DocumentValidation);

            TypeParser wrapperTypeParser = new TypeParser("atsex", wrapperTypes);
            TypeParser originalTypeParser = new TypeParser("bve", originalTypes);

            string targetNamespace = $"{{{schema.TargetNamespace}}}";

            XElement root = doc.Element(targetNamespace + "WrapTypes");
            MemberLoader loader = new MemberLoader(root, targetNamespace, wrapperTypeParser, originalTypeParser, additionalWrapperToOriginal);
            loader.LoadAll();

            return loader.Types;
        }

        private static void SchemaValidation(object sender, ValidationEventArgs e)
        {
            throw new FormatException(Resources.XmlSchemaValidation.Value, e.Exception);
        }

        private static void DocumentValidation(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }
    }
}