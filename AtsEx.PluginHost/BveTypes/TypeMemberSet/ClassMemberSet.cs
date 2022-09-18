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
        private protected static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<ClassMemberSet>("PluginHost");

        private protected SortedList<Type[], ConstructorInfo> Constructors { get; }

        private protected SortedList<string, MethodInfo> PropertyGetters { get; }
        private protected SortedList<string, MethodInfo> PropertySetters { get; }
        private protected SortedList<string, FieldInfo> Fields { get; }
        private protected SortedList<(string, Type[]), MethodInfo> Methods { get; }

        internal ClassMemberSet(Type wrapperType, Type originalType, SortedList<Type[], ConstructorInfo> constructors,
            SortedList<string, MethodInfo> propertyGetters, SortedList<string, MethodInfo> propertySetters, SortedList<string, FieldInfo> fields, SortedList<(string, Type[]), MethodInfo> methods)
            : base(wrapperType, originalType)
        {
            Constructors = constructors;

            PropertyGetters = propertyGetters;
            PropertySetters = propertySetters;
            Fields = fields;
            Methods = methods;
        }

        internal static ClassMemberSet FromTypeCollection(TypeInfo src, SortedList<Type[], ConstructorInfo> constructors,
            SortedList<string, MethodInfo> propertyGetters, SortedList<string, MethodInfo> propertySetters, SortedList<string, FieldInfo> fields, SortedList<(string, Type[]), MethodInfo> methods)
        {
            return new ClassMemberSet(src.WrapperType, src.OriginalType, constructors, propertyGetters, propertySetters, fields, methods);
        }

        public MethodInfo GetSourcePropertyGetterOf(string wrapperName)
        {
            if (!PropertyGetters.Keys.Contains(wrapperName))
            {
                throw new KeyNotFoundException(string.Format(Resources.GetString("OriginalPropertyNotFound").Value, nameof(wrapperName), wrapperName));
            }

            return PropertyGetters[wrapperName];
        }

        public MethodInfo GetSourcePropertySetterOf(string wrapperName)
        {
            if (!PropertySetters.Keys.Contains(wrapperName))
            {
                throw new KeyNotFoundException(string.Format(Resources.GetString("OriginalPropertyNotFound").Value, nameof(wrapperName), wrapperName));
            }

            return PropertySetters[wrapperName];
        }

        public FieldInfo GetSourceFieldOf(string wrapperName)
        {
            if (!Fields.Keys.Contains(wrapperName))
            {
                throw new KeyNotFoundException(string.Format(Resources.GetString("OriginalFieldNotFound").Value, nameof(wrapperName), wrapperName));
            }

            return Fields[wrapperName];
        }

        public ConstructorInfo GetSourceConstructor(Type[] parameters = null)
        {
            ConstructorInfo matchConstructor = Constructors.FirstOrDefault(x => parameters is null || x.Key.SequenceEqual(parameters)).Value;
            if (matchConstructor is null)
            {
                throw new KeyNotFoundException(string.Format(Resources.GetString("OriginalConstructorNotFound").Value, nameof(parameters), parameters));
            }

            return matchConstructor;
        }

        public MethodInfo GetSourceMethodOf(string wrapperName, Type[] parameters = null)
        {
            MethodInfo matchMethod = Methods.FirstOrDefault(x => x.Key.Item1 == wrapperName && (parameters is null || x.Key.Item2.SequenceEqual(parameters))).Value;
            if (matchMethod is null)
            {
                throw new KeyNotFoundException(string.Format(Resources.GetString("OriginalMethodNotFound").Value, nameof(wrapperName), wrapperName, nameof(parameters), parameters));
            }

            return matchMethod;
        }
    }
}
