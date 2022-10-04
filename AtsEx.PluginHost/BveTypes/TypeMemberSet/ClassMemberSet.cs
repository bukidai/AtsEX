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

        private readonly IReadOnlyDictionary<string, FastMethod> PropertyGetters;
        private readonly IReadOnlyDictionary<string, FastMethod> PropertySetters;
        private readonly IReadOnlyDictionary<string, FastField> Fields;
        private readonly IReadOnlyDictionary<Type[], FastConstructor> Constructors;
        private readonly IReadOnlyDictionary<(string Name, Type[] Parameters), FastMethod> Methods;

        internal ClassMemberSet(Type wrapperType, Type originalType,
            IReadOnlyDictionary<string, FastMethod> propertyGetters, IReadOnlyDictionary<string, FastMethod> propertySetters, IReadOnlyDictionary<string, FastField> fields,
            IReadOnlyDictionary<Type[], FastConstructor> constructors, IReadOnlyDictionary<(string, Type[]), FastMethod> methods)
            : base(wrapperType, originalType)
        {
            Constructors = constructors;

            PropertyGetters = propertyGetters;
            PropertySetters = propertySetters;
            Fields = fields;
            Methods = methods;
        }

        public FastMethod GetSourcePropertyGetterOf(string wrapperName)
        {
            return PropertyGetters.TryGetValue(wrapperName, out FastMethod method)
                ? method
                : throw new KeyNotFoundException(string.Format(Resources.OriginalPropertyNotFound.Value, nameof(wrapperName), wrapperName));
        }

        public FastMethod GetSourcePropertySetterOf(string wrapperName)
        {
            return PropertySetters.TryGetValue(wrapperName, out FastMethod method)
                ? method
                : throw new KeyNotFoundException(string.Format(Resources.OriginalPropertyNotFound.Value, nameof(wrapperName), wrapperName));
        }

        public FastField GetSourceFieldOf(string wrapperName)
        {
            return Fields.TryGetValue(wrapperName, out FastField field)
                ? field
                : throw new KeyNotFoundException(string.Format(Resources.OriginalFieldNotFound.Value, nameof(wrapperName), wrapperName));
        }

        public FastConstructor GetSourceConstructor(Type[] parameters = null)
        {
            FastConstructor matchConstructor = Constructors.FirstOrDefault(x => parameters is null || x.Key.SequenceEqual(parameters)).Value;

            return matchConstructor is null
                ? throw new KeyNotFoundException(string.Format(Resources.OriginalConstructorNotFound.Value, nameof(parameters), parameters))
                : matchConstructor;
        }

        public FastMethod GetSourceMethodOf(string wrapperName, Type[] parameters = null)
        {
            FastMethod matchMethod = Methods.FirstOrDefault(x => x.Key.Name == wrapperName && (parameters is null || x.Key.Parameters.SequenceEqual(parameters))).Value;

            return matchMethod is null
                ? throw new KeyNotFoundException(string.Format(Resources.OriginalMethodNotFound.Value, nameof(wrapperName), wrapperName, nameof(parameters), parameters))
                : matchMethod;
        }
    }
}
