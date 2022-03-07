using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.BveTypeCollection
{
    internal static class BveTypeNameDefinitionLoader
    {
        public static IEnumerable<TypeMemberNameCollection> LoadFile(Stream docStream, Stream schemaStream)
        {
            XDocument doc = XDocument.Load(docStream);

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            XmlSchema schema = XmlSchema.Read(schemaStream, SchemaValidation);
            schemaSet.Add(schema);
            doc.Validate(schemaSet, DocumentValidation);

            string targetNamespace = $"{{{schema.TargetNamespace}}}";

            var a = doc.Elements();

            XElement root = doc.Element(targetNamespace + "BveTypeNameDefinitions");
            IEnumerable<XElement> types = root.Elements(targetNamespace + "BveType");
            foreach (XElement type in types)
            {
                string typeWrapperName = (string)type.Attribute("Wrapper");
                string typeOriginalName = (string)type.Attribute("Original");

                TypeMemberNameCollection memberCollection = new TypeMemberNameCollection(typeWrapperName, typeOriginalName);

                {
                    IEnumerable<XElement> properties = type.Elements(targetNamespace + "Property");
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
                            string staticWrapperContainer = (string)getter.Attribute("StaticWrapperContainer");

                            if (isWrapperStatic && staticWrapperContainer is null)
                            {
                                throw new Exception($"静的プロパティ '{typeWrapperName}.{wrapperName}' のコンテナ型が定義されていません。");
                            }

                            TypeMemberNameCollection.PropertyAccessorInfo getterInfo =
                                new TypeMemberNameCollection.PropertyAccessorInfo(wrapperName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate, staticWrapperContainer);
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
                            string staticWrapperContainer = (string)setter.Attribute("StaticWrapperContainer");

                            if (isWrapperStatic && staticWrapperContainer is null)
                            {
                                throw new Exception($"静的プロパティ '{typeWrapperName}.{wrapperName}' のコンテナ型が定義されていません。");
                            }

                            TypeMemberNameCollection.PropertyAccessorInfo setterInfo =
                                new TypeMemberNameCollection.PropertyAccessorInfo(wrapperName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate, staticWrapperContainer);
                            memberCollection.PropertySetters.Add(setterInfo);
                        }

                        if (getter is null && setter is null)
                        {
                            throw new FormatException($"プロパティ '{typeWrapperName}.{wrapperName}' について、set / get いずれのアクセサも定義されていません。");
                        }
                    }
                }

                {
                    IEnumerable<XElement> methods = type.Elements(targetNamespace + "Method");
                    foreach (XElement method in methods)
                    {
                        string wrapperName = (string)method.Attribute("Wrapper");
                        IEnumerable<TypeMemberNameCollection.TypeInfoBase> wrapperParamNames = ParseParamArrayText((string)method.Attribute("WrapperParams"));
                        string originalName = (string)method.Attribute("Original");

                        bool isOriginalStatic = (bool?)method.Attribute("IsOriginalStatic") ?? false;
                        bool isOriginalPrivate = (bool?)method.Attribute("IsOriginalPrivate") ?? false;
                        bool isWrapperStatic = (bool?)method.Attribute("IsWrapperStatic") ?? false;
                        bool isWrapperPrivate = (bool?)method.Attribute("IsWrapperPrivate") ?? false;
                        string staticWrapperContainer = (string)method.Attribute("StaticWrapperContainer");

                        if (isWrapperStatic && staticWrapperContainer is null)
                        {
                            throw new Exception($"静的メソッド '{typeWrapperName}.{wrapperName}' のコンテナ型が定義されていません。");
                        }

                        TypeMemberNameCollection.MethodInfo methodInfo =
                            new TypeMemberNameCollection.MethodInfo(wrapperName, originalName, wrapperParamNames, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate, staticWrapperContainer);
                        memberCollection.Methods.Add(methodInfo);
                    }
                }

                {
                    IEnumerable<XElement> fields = type.Elements(targetNamespace + "Field");
                    foreach (XElement field in fields)
                    {
                        string wrapperPropertyName = (string)field.Attribute("WrapperProperty");
                        string originalName = (string)field.Attribute("Original");

                        bool isOriginalStatic = (bool?)field.Attribute("IsOriginalStatic") ?? false;
                        bool isOriginalPrivate = (bool?)field.Attribute("IsOriginalPrivate") ?? false;
                        bool isWrapperStatic = (bool?)field.Attribute("IsWrapperStatic") ?? false;
                        bool isWrapperPrivate = (bool?)field.Attribute("IsWrapperPrivate") ?? false;
                        string staticWrapperContainer = (string)field.Attribute("StaticWrapperContainer");

                        if (isWrapperStatic && staticWrapperContainer is null)
                        {
                            throw new Exception($"静的フィールド '{typeWrapperName}.{wrapperPropertyName}' のコンテナ型が定義されていません。");
                        }

                        TypeMemberNameCollection.FieldInfo fieldInfo =
                            new TypeMemberNameCollection.FieldInfo(wrapperPropertyName, originalName, isOriginalStatic, isOriginalPrivate, isWrapperStatic, isWrapperPrivate, staticWrapperContainer);
                        memberCollection.Fields.Add(fieldInfo);
                    }
                }

                yield return memberCollection;
            }


            IEnumerable<TypeMemberNameCollection.TypeInfoBase> ParseParamArrayText(string text)
            {
                switch (text)
                {
                    case null:
                    case "":
                    case "void":
                    case "System.Void":
                        return Enumerable.Empty<TypeMemberNameCollection.TypeInfoBase>();

                    default:
                        return TypeTextParser.SplitArrayText(text).Select(TypeTextParser.Parse);
                }
            }
        }

        static void SchemaValidation(object sender, ValidationEventArgs e)
        {
            throw new DevelopException("BVE 型・メンバー名定義 XML のスキーマが不正なフォーマットです。", e.Exception);
        }

        static void DocumentValidation(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }
    }
}
