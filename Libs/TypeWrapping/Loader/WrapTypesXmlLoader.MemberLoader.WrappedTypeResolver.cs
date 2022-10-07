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
            private class WrappedTypeResolver : TypeLoaderBase
            {
                private class ResourceSet
                {
                    private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<WrappedTypeResolver>(@"TypeWrapping\WrapTypesXmlLoader");

                    [ResourceStringHolder(nameof(Localizer))] public Resource<string> ElementTypesButArrayNotSupported { get; private set; }

                    public ResourceSet()
                    {
                        ResourceLoader.LoadAndSetAll(this);
                    }
                }

                private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

                static WrappedTypeResolver()
                {
#if DEBUG
                    _ = Resources.Value;
#endif
                }

                private readonly Dictionary<Type, Type> WrapperToOriginal = new Dictionary<Type, Type>();

                private readonly TypeParser WrapperTypeParser;
                private readonly TypeParser OriginalTypeParser;

                public int WrappedTypeCount => IsLoaded ? WrapperToOriginal.Count : throw new InvalidOperationException();

                public WrappedTypeResolver(XElement root, string targetNamespace,
                    TypeParser wrapperTypeParser, TypeParser originalTypeParser, IDictionary<Type, Type> additionalWrapperToOriginal) : base(root, targetNamespace)
                {
                    WrapperTypeParser = wrapperTypeParser;
                    OriginalTypeParser = originalTypeParser;

                    WrapperToOriginal = new Dictionary<Type, Type>(additionalWrapperToOriginal);
                }

                protected override void LoadEnums(IEnumerable<XElement> enumElements, IEnumerable<XElement> parentClassElements) => Load(enumElements, parentClassElements);

                protected override void LoadClasses(IEnumerable<XElement> classElements, IEnumerable<XElement> parentClassElements) => Load(classElements, parentClassElements);

                private void Load(IEnumerable<XElement> elements, IEnumerable<XElement> parentClassElements)
                {
                    string parentWrapperClassName = GetParentWrapperClassName(parentClassElements);
                    string parentOriginalClassName = GetParentOriginalClassName(parentClassElements);


                    elements.AsParallel().ForAll(element =>
                    {
                        string wrapperTypeName = GetWrapperTypeName(element);
                        string originalTypeName = GetOriginalTypeName(element);

                        Type wrapperType = WrapperTypeParser.ParseSingleSpecializedTypeName(parentWrapperClassName + wrapperTypeName);
                        Type originalType = OriginalTypeParser.ParseSingleSpecializedTypeName(parentOriginalClassName + originalTypeName);

                        lock (WrapperToOriginal) WrapperToOriginal.Add(wrapperType, originalType);
                    });
                }

                public Type GetOriginal(Type wrapper)
                {
                    if (wrapper.IsConstructedGenericType)
                    {
                        Type wrapperParent = wrapper.GetGenericTypeDefinition();
                        WrapperToOriginal.TryGetValue(wrapperParent, out Type originalParent);
                        originalParent = originalParent ?? wrapperParent;

                        Type[] wrapperChildren = wrapper.GetGenericArguments();
                        Type[] originalChildren = new Type[wrapperChildren.Length];
                        for (int i = 0; i < wrapperChildren.Length; i++)
                        {
                            WrapperToOriginal.TryGetValue(wrapperChildren[i], out Type originalChild);
                            originalChildren[i] = originalChild ?? wrapperChildren[i];
                        }

                        Type result = originalParent.MakeGenericType(originalChildren);
                        return result;
                    }
                    else if (wrapper.HasElementType)
                    {
                        if (!wrapper.IsArray || wrapper.Name.Contains("*")) throw new NotSupportedException(Resources.Value.ElementTypesButArrayNotSupported.Value);

                        Type wrapperElement = wrapper.GetElementType();
                        WrapperToOriginal.TryGetValue(wrapperElement, out Type originalElement);
                        originalElement = originalElement ?? wrapperElement;

                        int rank = wrapper.GetArrayRank();
                        Type result = rank == 1 ? originalElement.MakeArrayType() : originalElement.MakeArrayType(rank);
                        return result;
                    }
                    else
                    {
                        WrapperToOriginal.TryGetValue(wrapper, out Type result);
                        return result ?? wrapper;
                    }
                }

                public (Type Wrapper, Type Original) Resolve(string wrapperName)
                {
                    Type wrapper = WrapperTypeParser.ParseSingleSpecializedTypeName(wrapperName);
                    Type original = GetOriginal(wrapper);

                    return (wrapper, original);
                }

                public (Type Wrapper, Type Original) Resolve(XElement typeElement, IEnumerable<XElement> parentClassElements)
                {
                    string wrapperName = GetWrapperTypeName(typeElement);
                    string parentWrapperClassName = GetParentWrapperClassName(parentClassElements);

                    (Type Wrapper, Type Original) typePair = Resolve(parentWrapperClassName + wrapperName);
                    return typePair;
                }


                private static string GetWrapperTypeName(XElement element) => GetTypeName(element, "Wrapper");
                private static string GetOriginalTypeName(XElement element) => GetTypeName(element, "Original");

                private static string GetTypeName(XElement element, string attributeName)
                {
                    string typeName = (string)element.Attribute(attributeName);
                    return typeName;
                }


                private static string GetParentWrapperClassName(IEnumerable<XElement> parentClassElements) => GetParentClassName(parentClassElements, "Wrapper");
                private static string GetParentOriginalClassName(IEnumerable<XElement> parentClassElements) => GetParentClassName(parentClassElements, "Original");

                private static string GetParentClassName(IEnumerable<XElement> parentClassElements, string attributeName)
                {
                    StringBuilder parentClassNameBuilder = new StringBuilder();

                    foreach (XElement parentClassElement in parentClassElements)
                    {
                        string wrapperTypeName = GetTypeName(parentClassElement, attributeName);

                        parentClassNameBuilder.Append(wrapperTypeName);
                        parentClassNameBuilder.Append("+");
                    }

                    string parentClassName = parentClassNameBuilder.ToString();
                    return parentClassName;
                }
            }
        }
    }
}
