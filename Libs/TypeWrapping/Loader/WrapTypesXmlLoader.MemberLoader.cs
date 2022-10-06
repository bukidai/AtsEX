using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using FastMember;
using UnembeddedResources;

namespace TypeWrapping
{
    public static partial class WrapTypesXmlLoader
    {
        private partial class MemberLoader : TypeLoaderBase
        {
            private class ResourceSet
            {
                private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<MemberLoader>(@"TypeWrapping\WrapTypesXmlLoader");

                [ResourceStringHolder(nameof(Localizer))] public Resource<string> PropertyImplementationInvalid { get; private set; }

                public ResourceSet()
                {
                    ResourceLoader.LoadAndSetAll(this);
                }
            }

            private static readonly ResourceSet Resources = new ResourceSet();

            public List<TypeMemberSetBase> Types { get; }

            private readonly TypeParser WrapperTypeParser;
            private readonly WrappedTypeResolver Resolver;

            public MemberLoader(XElement root, string targetNamespace,
                TypeParser wrapperTypeParser, TypeParser originalTypeParser, IDictionary<Type, Type> additionalWrapperToOriginal) : base(root, targetNamespace)
            {
                WrapperTypeParser = wrapperTypeParser;

                Resolver = new WrappedTypeResolver(root, targetNamespace, wrapperTypeParser, originalTypeParser, additionalWrapperToOriginal);
                Resolver.LoadAll();

                Types = new List<TypeMemberSetBase>(Resolver.WrappedTypeCount);
            }

            protected override void LoadEnums(IEnumerable<XElement> enumElements, IEnumerable<XElement> parentClassElements)
            {
                IEnumerable<TypeMemberSetBase> types = enumElements.AsParallel().Select(element =>
                {
                    (Type wrapperType, Type originalType) = Resolver.Resolve(element, parentClassElements);
                    EnumMemberSet members = new EnumMemberSet(wrapperType, originalType);

                    return members;
                });

                Types.AddRange(types);
            }

            protected override void LoadClasses(IEnumerable<XElement> classElements, IEnumerable<XElement> parentClassElements)
            {
                IEnumerable<TypeMemberSetBase> loadedTypes = classElements.AsParallel().Select(classElement =>
                {
                    (Type wrapperType, Type originalType) = Resolver.Resolve(classElement, parentClassElements);
                    ElementParser elementParser = new ElementParser(wrapperType, originalType);

                    Dictionary<string, FastMethod> propertyGetters = new Dictionary<string, FastMethod>();
                    Dictionary<string, FastMethod> propertySetters = new Dictionary<string, FastMethod>();
                    Dictionary<string, FastField> fields = new Dictionary<string, FastField>();
                    Dictionary<Type[], FastConstructor> constructors = new Dictionary<Type[], FastConstructor>();
                    Dictionary<(string, Type[]), FastMethod> methods = new Dictionary<(string, Type[]), FastMethod>();

                    {
                        IEnumerable<XElement> propertyElements = classElement.Elements(TargetNamespace + "Property");
                        foreach (XElement propertyElement in propertyElements)
                        {
                            PropertyInfo wrapperProperty = elementParser.GetWrapperProperty(propertyElement);

                            XElement getter = propertyElement.Element(TargetNamespace + "Getter");
                            if (!(getter is null))
                            {
                                MethodInfo originalGetter = elementParser.GetOriginalMethod(getter, Type.EmptyTypes);
                                propertyGetters.Add(wrapperProperty.Name, FastMethod.Create(originalGetter));
                            }

                            XElement setter = propertyElement.Element(TargetNamespace + "Setter");
                            if (!(setter is null))
                            {
                                MethodInfo originalSetter = elementParser.GetOriginalMethod(setter, new Type[] { Resolver.GetOriginal(wrapperProperty.PropertyType) });
                                propertySetters.Add(wrapperProperty.Name, FastMethod.Create(originalSetter));
                            }

                            if (getter is null && setter is null)
                            {
                                throw new FormatException(string.Format(Resources.PropertyImplementationInvalid.Value, $"{wrapperType.Name}.{wrapperProperty.Name}"));
                            }
                        }
                    }

                    if (!originalType.IsAbstract)
                    {
                        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.InvokeMethod;
                        ConstructorInfo[] wrapperConstructors = wrapperType.GetConstructors(bindingFlags);
                        foreach (ConstructorInfo wrapperConstructor in wrapperConstructors)
                        {
                            Type[] wrapperParamTypes = wrapperConstructor.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
                            Type[] originalParamTypes = wrapperParamTypes.Select(Resolver.GetOriginal).ToArray();

                            ConstructorInfo originalConstructor = originalType.GetConstructor(originalParamTypes);
                            if (originalConstructor is null) continue;

                            constructors.Add(wrapperParamTypes, FastConstructor.Create(originalConstructor));
                        }
                    }

                    {
                        IEnumerable<XElement> methodElements = classElement.Elements(TargetNamespace + "Method");
                        foreach (XElement methodElement in methodElements)
                        {
                            string wrapperParamsText = (string)methodElement.Attribute("WrapperParams");

                            Type[] wrapperParamTypes = WrapperTypeParser.ParseArray(wrapperParamsText).ToArray();
                            Type[] originalParamTypes = wrapperParamTypes.Select(Resolver.GetOriginal).ToArray();

                            MethodInfo wrapperMethod = elementParser.GetWrapperMethod(methodElement, wrapperParamTypes);
                            MethodInfo originalMethod = elementParser.GetOriginalMethod(methodElement, originalParamTypes);

                            methods.Add((wrapperMethod.Name, wrapperParamTypes), FastMethod.Create(originalMethod));
                        }
                    }

                    {
                        IEnumerable<XElement> fieldElements = classElement.Elements(TargetNamespace + "Field");
                        foreach (XElement fieldElement in fieldElements)
                        {
                            PropertyInfo fieldWrapperProperty = elementParser.GetFieldWrapperProperty(fieldElement);
                            FieldInfo originalField = elementParser.GetOriginalField(fieldElement);

                            fields.Add(fieldWrapperProperty.Name, FastField.Create(originalField));
                        }
                    }

                    ClassMemberSet members = new ClassMemberSet(wrapperType, originalType, propertyGetters, propertySetters, fields, constructors, methods);
                    return members;
                });

                Types.AddRange(loadedTypes);
            }
        }
    }
}
