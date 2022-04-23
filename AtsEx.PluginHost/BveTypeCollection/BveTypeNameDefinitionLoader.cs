using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    internal static class BveTypeNameDefinitionLoader
    {
        public static IEnumerable<TypeMemberNameCollectionBase> LoadFile(Stream docStream, Stream schemaStream)
        {
            XDocument doc = XDocument.Load(docStream);

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            XmlSchema schema = XmlSchema.Read(schemaStream, SchemaValidation);
            schemaSet.Add(schema);
            doc.Validate(schemaSet, DocumentValidation);

            string targetNamespace = $"{{{schema.TargetNamespace}}}";

            XElement root = doc.Element(targetNamespace + "BveTypeNameDefinitions");

            IEnumerable<XElement> enumElements = root.Elements(targetNamespace + "Enum");
            foreach (XElement enumElement in enumElements)
            {
                string typeWrapperName = (string)enumElement.Attribute("Wrapper");
                string typeOriginalName = (string)enumElement.Attribute("Original");

                EnumMemberNameCollection memberCollection = new EnumMemberNameCollection(typeWrapperName, typeOriginalName);
                yield return memberCollection;
            }

            IEnumerable<XElement> classElements = root.Elements(targetNamespace + "Class");
            foreach (XElement classElement in classElements)
            {
                string typeWrapperName = (string)classElement.Attribute("Wrapper");
                string typeOriginalName = (string)classElement.Attribute("Original");

                ClassMemberNameCollection memberCollection = new ClassMemberNameCollection(typeWrapperName, typeOriginalName);

                {
                    IEnumerable<XElement> properties = classElement.Elements(targetNamespace + "Property");
                    foreach (XElement property in properties)
                    {
                        string wrapperName = (string)property.Attribute("Wrapper");

                        XElement getter = property.Element(targetNamespace + "Getter");
                        if (!(getter is null))
                        {
                            string originalName = (string)getter.Attribute("Original");

                            bool isOriginalStatic = (bool?)getter.Attribute("IsOriginalStatic") ?? false;
                            bool isOriginalPrivate = (bool?)getter.Attribute("IsOriginalPrivate") ?? false;
                            bool isWrapperStatic = (bool?)getter.Attribute("IsWrapperStatic") ?? false;
                            bool isWrapperPrivate = (bool?)getter.Attribute("IsWrapperPrivate") ?? false;

                            TypeMemberNameCollectionBase.PropertyAccessorInfo getterInfo =
                                new TypeMemberNameCollectionBase.PropertyAccessorInfo(wrapperName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate);
                            memberCollection.PropertyGetters.Add(getterInfo);
                        }

                        XElement setter = property.Element(targetNamespace + "Setter");
                        if (!(setter is null))
                        {
                            string originalName = (string)setter.Attribute("Original");

                            bool isOriginalStatic = (bool?)setter.Attribute("IsOriginalStatic") ?? false;
                            bool isOriginalPrivate = (bool?)setter.Attribute("IsOriginalPrivate") ?? false;
                            bool isWrapperStatic = (bool?)setter.Attribute("IsWrapperStatic") ?? false;
                            bool isWrapperPrivate = (bool?)setter.Attribute("IsWrapperPrivate") ?? false;

                            TypeMemberNameCollectionBase.PropertyAccessorInfo setterInfo =
                                new TypeMemberNameCollectionBase.PropertyAccessorInfo(wrapperName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate);
                            memberCollection.PropertySetters.Add(setterInfo);
                        }

                        if (getter is null && setter is null)
                        {
                            throw new FormatException($"プロパティ '{typeWrapperName}.{wrapperName}' について、set / get いずれのアクセサも定義されていません。");
                        }
                    }
                }

                {
                    IEnumerable<XElement> methods = classElement.Elements(targetNamespace + "Method");
                    foreach (XElement method in methods)
                    {
                        string wrapperName = (string)method.Attribute("Wrapper");
                        IEnumerable<TypeMemberNameCollectionBase.TypeInfoBase> wrapperParamNames = ParseParamArrayText((string)method.Attribute("WrapperParams"));
                        string originalName = (string)method.Attribute("Original");

                        bool isOriginalStatic = (bool?)method.Attribute("IsOriginalStatic") ?? false;
                        bool isOriginalPrivate = (bool?)method.Attribute("IsOriginalPrivate") ?? false;
                        bool isWrapperStatic = (bool?)method.Attribute("IsWrapperStatic") ?? false;
                        bool isWrapperPrivate = (bool?)method.Attribute("IsWrapperPrivate") ?? false;

                        TypeMemberNameCollectionBase.MethodInfo methodInfo =
                            new TypeMemberNameCollectionBase.MethodInfo(wrapperName, originalName, wrapperParamNames, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate);
                        memberCollection.Methods.Add(methodInfo);
                    }
                }

                {
                    IEnumerable<XElement> fields = classElement.Elements(targetNamespace + "Field");
                    foreach (XElement field in fields)
                    {
                        string wrapperPropertyName = (string)field.Attribute("WrapperProperty");
                        string originalName = (string)field.Attribute("Original");

                        bool isOriginalStatic = (bool?)field.Attribute("IsOriginalStatic") ?? false;
                        bool isOriginalPrivate = (bool?)field.Attribute("IsOriginalPrivate") ?? false;
                        bool isWrapperStatic = (bool?)field.Attribute("IsWrapperStatic") ?? false;
                        bool isWrapperPrivate = (bool?)field.Attribute("IsWrapperPrivate") ?? false;

                        TypeMemberNameCollectionBase.FieldInfo fieldInfo =
                            new TypeMemberNameCollectionBase.FieldInfo(wrapperPropertyName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate);
                        memberCollection.Fields.Add(fieldInfo);
                    }
                }

                yield return memberCollection;
            }


            IEnumerable<TypeMemberNameCollectionBase.TypeInfoBase> ParseParamArrayText(string text)
            {
                switch (text)
                {
                    case null:
                    case "":
                    case "void":
                    case "System.Void":
                        return Enumerable.Empty<TypeMemberNameCollectionBase.TypeInfoBase>();

                    default:
                        return TypeTextParser.SplitArrayText(text).Select(TypeTextParser.Parse);
                }
            }
        }

        static void SchemaValidation(object sender, ValidationEventArgs e)
        {
            throw new FormatException("BVE 型・メンバー名定義 XML のスキーマが不正なフォーマットです。", e.Exception);
        }

        static void DocumentValidation(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }
    }
}
