using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using UnembeddedResources;

namespace TypeWrapping
{
    public static partial class WrapTypesXmlLoader
    {
        private partial class MemberLoader
        {
            private class ElementParser
            {
                private class ResourceSet
                {
                    private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<ElementParser>(@"TypeWrapping\WrapTypesXmlLoader");

                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> OriginalFieldNotFound { get; private set; }
                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> WrapperPropertyNotFound { get; private set; }
                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> FieldWrapperPropertyNotFound { get; private set; }
                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> WrapperMethodNotFound { get; private set; }
                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> OriginalMethodNotFound { get; private set; }
                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> NonPublic { get; private set; }
                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> Public { get; private set; }
                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> Static { get; private set; }
                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> Instance { get; private set; }

                    public ResourceSet()
                    {
                        ResourceLoader.LoadAndSetAll(this);
                    }
                }

                private static readonly ResourceSet Resources = new ResourceSet();

                private readonly Type WrapperType;
                private readonly Type OriginalType;

                private readonly MemberParser WrapperMemberParser;
                private readonly MemberParser OriginalMemberParser;

                public ElementParser(Type wrapperType, Type originalType)
                {
                    WrapperType = wrapperType;
                    OriginalType = originalType;

                    WrapperMemberParser = new MemberParser(WrapperType);
                    OriginalMemberParser = new MemberParser(OriginalType);
                }

                public FieldInfo GetOriginalField(XElement source)
                {
                    (string name, bool isNonPublic, bool isStatic) = GetOriginalInfo(source);
                    FieldInfo field = OriginalMemberParser.GetField(name, isNonPublic, isStatic);

                    return field ?? throw CreateNotFoundException(Resources.OriginalFieldNotFound, name, isNonPublic, isStatic);
                }

                public PropertyInfo GetWrapperProperty(XElement source)
                {
                    (string name, bool isNonPublic, bool isStatic) = GetWrapperInfo(source);
                    PropertyInfo property = WrapperMemberParser.GetProperty(name, isNonPublic, isStatic);

                    return property ?? throw CreateNotFoundException(Resources.WrapperPropertyNotFound, name, isNonPublic, isStatic);
                }

                public PropertyInfo GetFieldWrapperProperty(XElement source)
                {
                    (string name, bool isNonPublic, bool isStatic) = GetInfo(source, "WrapperProperty", "IsWrapperNonPublic", "IsWrapperStatic");
                    PropertyInfo property = WrapperMemberParser.GetProperty(name, isNonPublic, isStatic);

                    return property ?? throw CreateNotFoundException(Resources.FieldWrapperPropertyNotFound, name, isNonPublic, isStatic);
                }

                public MethodInfo GetWrapperMethod(XElement source, Type[] types)
                {
                    (string name, bool isNonPublic, bool isStatic) = GetWrapperInfo(source);
                    MethodInfo method = WrapperMemberParser.GetMethod(name, types, isNonPublic, isStatic);

                    return method ?? throw CreateNotFoundException(Resources.WrapperMethodNotFound, name, isNonPublic, isStatic);
                }

                public MethodInfo GetOriginalMethod(XElement source, Type[] types)
                {
                    (string name, bool isNonPublic, bool isStatic) = GetOriginalInfo(source);
                    MethodInfo method = OriginalMemberParser.GetMethod(name, types, isNonPublic, isStatic);

                    return method ?? throw CreateNotFoundException(Resources.OriginalMethodNotFound, name, isNonPublic, isStatic);
                }

                private KeyNotFoundException CreateNotFoundException(Resource<string> exceptionTextResource, string name, bool isNonPublic, bool isStatic)
                {
                    throw new KeyNotFoundException(string.Format(exceptionTextResource.Value, GetAccessibilityDescription(), name, WrapperType.Name));

                    string GetAccessibilityDescription()
                        => (isNonPublic ? Resources.NonPublic : Resources.Public).Value + (isStatic ? Resources.Static : Resources.Instance).Value;
                }


                private (string Name, bool IsNonPublic, bool IsStatic) GetWrapperInfo(XElement element) => GetInfo(element, "Wrapper", "IsWrapperNonPublic", "IsWrapperStatic");
                private (string Name, bool IsNonPublic, bool IsStatic) GetOriginalInfo(XElement element) => GetInfo(element, "Original", "IsOriginalNonPublic", "IsOriginalStatic");

                private (string Name, bool IsNonPublic, bool IsStatic) GetInfo(XElement element, string nameAttributeName, string isNonPublicAttributeName, string isStaticAttributeName)
                {
                    string name = (string)element.Attribute(nameAttributeName);

                    bool isNonPublic = (bool?)element.Attribute(isNonPublicAttributeName) ?? false;
                    bool isStatic = (bool?)element.Attribute(isStaticAttributeName) ?? false;

                    return (name, isNonPublic, isStatic);
                }
            }
        }
    }
}
