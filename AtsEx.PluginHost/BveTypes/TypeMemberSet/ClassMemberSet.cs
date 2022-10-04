using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    public class ClassMemberSet : TypeMemberSetBase
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<ClassMemberSet>("PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> OriginalPropertyNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> OriginalFieldNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> OriginalConstructorNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> OriginalMethodNotFound { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly ResourceSet Resources = new ResourceSet();

        private readonly IReadOnlyDictionary<string, MethodInfo> PropertyGetters;
        private readonly IReadOnlyDictionary<string, MethodInfo> PropertySetters;
        private readonly IReadOnlyDictionary<string, FieldInfo> Fields;
        private readonly IReadOnlyDictionary<Type[], ConstructorInfo> Constructors;
        private readonly IReadOnlyDictionary<(string Name, Type[] Parameters), MethodInfo> Methods;

        internal ClassMemberSet(Type wrapperType, Type originalType,
            IReadOnlyDictionary<string, MethodInfo> propertyGetters, IReadOnlyDictionary<string, MethodInfo> propertySetters, IReadOnlyDictionary<string, FieldInfo> fields,
            IReadOnlyDictionary<Type[], ConstructorInfo> constructors, IReadOnlyDictionary<(string, Type[]), MethodInfo> methods)
            : base(wrapperType, originalType)
        {
            Constructors = constructors;

            PropertyGetters = propertyGetters;
            PropertySetters = propertySetters;
            Fields = fields;
            Methods = methods;
        }

        public MethodInfo GetSourcePropertyGetterOf(string wrapperName)
        {
            return PropertyGetters.TryGetValue(wrapperName, out MethodInfo method)
                ? method
                : throw new KeyNotFoundException(string.Format(Resources.OriginalPropertyNotFound.Value, nameof(wrapperName), wrapperName));
        }

        public MethodInfo GetSourcePropertySetterOf(string wrapperName)
        {
            return PropertySetters.TryGetValue(wrapperName, out MethodInfo method)
                ? method
                : throw new KeyNotFoundException(string.Format(Resources.OriginalPropertyNotFound.Value, nameof(wrapperName), wrapperName));
        }

        public FieldInfo GetSourceFieldOf(string wrapperName)
        {
            return Fields.TryGetValue(wrapperName, out FieldInfo field)
                ? field
                : throw new KeyNotFoundException(string.Format(Resources.OriginalFieldNotFound.Value, nameof(wrapperName), wrapperName));
        }

        public ConstructorInfo GetSourceConstructor(Type[] parameters = null)
        {
            ConstructorInfo matchConstructor = Constructors.FirstOrDefault(x => parameters is null || x.Key.SequenceEqual(parameters)).Value;

            return matchConstructor is null
                ? throw new KeyNotFoundException(string.Format(Resources.OriginalConstructorNotFound.Value, nameof(parameters), parameters))
                : matchConstructor;
        }

        public MethodInfo GetSourceMethodOf(string wrapperName, Type[] parameters = null)
        {
            MethodInfo matchMethod = Methods.FirstOrDefault(x => x.Key.Name == wrapperName && (parameters is null || x.Key.Parameters.SequenceEqual(parameters))).Value;

            return matchMethod is null
                ? throw new KeyNotFoundException(string.Format(Resources.OriginalMethodNotFound.Value, nameof(wrapperName), wrapperName, nameof(parameters), parameters))
                : matchMethod;
        }
    }
}
