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

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    internal static class WrapTypesXmlLoader
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType(typeof(WrapTypesXmlLoader), "PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PropertyImplementationInvalid { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> XmlSchemaValidation { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly ResourceSet Resources = new ResourceSet();

        public static List<TypeMemberNameSetBase> LoadFile(Stream docStream, Stream schemaStream)
        {
            XDocument doc = XDocument.Load(docStream);

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            XmlSchema schema = XmlSchema.Read(schemaStream, SchemaValidation);
            schemaSet.Add(schema);
            doc.Validate(schemaSet, DocumentValidation);

            string targetNamespace = $"{{{schema.TargetNamespace}}}";

            XElement root = doc.Element(targetNamespace + "WrapTypes");
            return LoadChildTypes(root);


            List<TypeMemberNameSetBase> LoadChildTypes(XElement parent)
            {
                List<TypeMemberNameSetBase> memberCollections = new List<TypeMemberNameSetBase>();

                {
                    IEnumerable<XElement> elements = parent.Elements(targetNamespace + "Enum");
                    memberCollections.AddRange(LoadEnums(elements));
                }

                {
                    IEnumerable<XElement> elements = parent.Elements(targetNamespace + "Class");
                    memberCollections.AddRange(LoadClasses(elements));
                }

                return memberCollections;
            }


            List<EnumMemberNameSet> LoadEnums(IEnumerable<XElement> elements)
            {
                List<EnumMemberNameSet> memberCollections = elements.AsParallel().Select(element =>
                {
                    string typeWrapperName = (string)element.Attribute("Wrapper");
                    string typeOriginalName = (string)element.Attribute("Original");

                    EnumMemberNameSet memberCollection = new EnumMemberNameSet(typeWrapperName, typeOriginalName);
                    return memberCollection;
                }).ToList();

                return memberCollections;
            }

            List<ClassMemberNameSet> LoadClasses(IEnumerable<XElement> elements)
            {
                List<ClassMemberNameSet> memberCollections = elements.AsParallel().Select(element =>
                {
                    string typeWrapperName = (string)element.Attribute("Wrapper");
                    string typeOriginalName = (string)element.Attribute("Original");

                    List<TypeMemberNameSetBase> children = LoadChildTypes(element);

                    ClassMemberNameSet memberCollection = new ClassMemberNameSet(typeWrapperName, typeOriginalName, children);

                    {
                        IEnumerable<XElement> properties = element.Elements(targetNamespace + "Property");
                        foreach (XElement property in properties)
                        {
                            string wrapperName = (string)property.Attribute("Wrapper");

                            bool isWrapperStatic = (bool?)property.Attribute("IsWrapperStatic") ?? false;
                            bool isWrapperPrivate = (bool?)property.Attribute("IsWrapperPrivate") ?? false;

                            XElement getter = property.Element(targetNamespace + "Getter");
                            if (!(getter is null))
                            {
                                string originalName = (string)getter.Attribute("Original");

                                bool isOriginalStatic = (bool?)getter.Attribute("IsOriginalStatic") ?? false;
                                bool isOriginalPrivate = (bool?)getter.Attribute("IsOriginalPrivate") ?? false;

                                TypeMemberNameSetBase.PropertyAccessor getterInfo =
                                    new TypeMemberNameSetBase.PropertyAccessor(wrapperName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate);
                                memberCollection.PropertyGetters.Add(getterInfo);
                            }

                            XElement setter = property.Element(targetNamespace + "Setter");
                            if (!(setter is null))
                            {
                                string originalName = (string)setter.Attribute("Original");

                                bool isOriginalStatic = (bool?)setter.Attribute("IsOriginalStatic") ?? false;
                                bool isOriginalPrivate = (bool?)setter.Attribute("IsOriginalPrivate") ?? false;

                                TypeMemberNameSetBase.PropertyAccessor setterInfo =
                                    new TypeMemberNameSetBase.PropertyAccessor(wrapperName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate);
                                memberCollection.PropertySetters.Add(setterInfo);
                            }

                            if (getter is null && setter is null)
                            {
                                throw new FormatException(string.Format(Resources.PropertyImplementationInvalid.Value, $"{typeWrapperName}.{wrapperName}"));
                            }
                        }
                    }

                    {
                        IEnumerable<XElement> methods = element.Elements(targetNamespace + "Method");
                        foreach (XElement method in methods)
                        {
                            string wrapperName = (string)method.Attribute("Wrapper");
                            IEnumerable<TypeMemberNameSetBase.TypeInfoBase> wrapperParamNames = ParseParamArrayText((string)method.Attribute("WrapperParams"));
                            string originalName = (string)method.Attribute("Original");

                            bool isOriginalStatic = (bool?)method.Attribute("IsOriginalStatic") ?? false;
                            bool isOriginalPrivate = (bool?)method.Attribute("IsOriginalPrivate") ?? false;
                            bool isWrapperStatic = (bool?)method.Attribute("IsWrapperStatic") ?? false;
                            bool isWrapperPrivate = (bool?)method.Attribute("IsWrapperPrivate") ?? false;

                            TypeMemberNameSetBase.Method methodInfo =
                                new TypeMemberNameSetBase.Method(wrapperName, originalName, wrapperParamNames, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate);
                            memberCollection.Methods.Add(methodInfo);
                        }
                    }

                    {
                        IEnumerable<XElement> fields = element.Elements(targetNamespace + "Field");
                        foreach (XElement field in fields)
                        {
                            string wrapperPropertyName = (string)field.Attribute("WrapperProperty");
                            string originalName = (string)field.Attribute("Original");

                            bool isOriginalStatic = (bool?)field.Attribute("IsOriginalStatic") ?? false;
                            bool isOriginalPrivate = (bool?)field.Attribute("IsOriginalPrivate") ?? false;
                            bool isWrapperStatic = (bool?)field.Attribute("IsWrapperStatic") ?? false;
                            bool isWrapperPrivate = (bool?)field.Attribute("IsWrapperPrivate") ?? false;

                            TypeMemberNameSetBase.Field fieldInfo =
                                new TypeMemberNameSetBase.Field(wrapperPropertyName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate);
                            memberCollection.Fields.Add(fieldInfo);
                        }
                    }

                    return memberCollection;
                }).ToList();

                return memberCollections;
            }


            IEnumerable<TypeMemberNameSetBase.TypeInfoBase> ParseParamArrayText(string text)
            {
                switch (text)
                {
                    case null:
                    case "":
                    case "void":
                    case "System.Void":
                        return Enumerable.Empty<TypeMemberNameSetBase.TypeInfoBase>();

                    default:
                        return TypeTextParser.SplitArrayText(text).Select(TypeTextParser.Parse);
                }
            }
        }

        static void SchemaValidation(object sender, ValidationEventArgs e)
        {
            throw new FormatException(Resources.XmlSchemaValidation.Value, e.Exception);
        }

        static void DocumentValidation(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }
    }
}
