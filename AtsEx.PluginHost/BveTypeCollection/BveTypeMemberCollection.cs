using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    public class BveTypeMemberCollection
    {
        public Type WrapperType { get; }
        public Type OriginalType { get; }

        protected SortedList<string, MethodInfo> PropertyGetters { get; }
        protected SortedList<string, MethodInfo> PropertySetters { get; }
        protected SortedList<string, FieldInfo> Fields { get; }
        protected SortedList<Type[], ConstructorInfo> Constructors { get; }
        protected SortedList<(string, Type[]), MethodInfo> Methods { get; }

        internal BveTypeMemberCollection(Type wrapperType, Type originalType,
            SortedList<string, MethodInfo> propertyGetters, SortedList<string, MethodInfo> propertySetters, SortedList<string, FieldInfo> fields, SortedList<(string, Type[]), MethodInfo> methods)
        {
            WrapperType = wrapperType;
            OriginalType = originalType;

            Constructors = new SortedList<Type[], ConstructorInfo>(WrapperType.GetConstructors().ToDictionary(c => c.GetParameters().Select(p => p.ParameterType).ToArray(), c => c));

            PropertyGetters = propertyGetters;
            PropertySetters = propertySetters;
            Fields = fields;
            Methods = methods;
        }

        public MethodInfo GetSourcePropertyGetterOf(string wrapperName)
        {
            if (!PropertyGetters.Keys.Contains(wrapperName))
            {
                throw new KeyNotFoundException($"{nameof(wrapperName)} '{wrapperName}' は無効なキーです。");
            }

            return PropertyGetters[wrapperName];
        }

        public MethodInfo GetSourcePropertySetterOf(string wrapperName)
        {
            if (!PropertySetters.Keys.Contains(wrapperName))
            {
                throw new KeyNotFoundException($"{nameof(wrapperName)} '{wrapperName}' は無効なキーです。");
            }

            return PropertySetters[wrapperName];
        }

        public FieldInfo GetSourceFieldOf(string wrapperName)
        {
            if (!Fields.Keys.Contains(wrapperName))
            {
                throw new KeyNotFoundException($"{nameof(wrapperName)} '{wrapperName}' は無効なキーです。");
            }

            return Fields[wrapperName];
        }

        public ConstructorInfo GetSourceConstructorOf(Type[] parameters = null)
        {
            ConstructorInfo matchConstructor = Constructors.FirstOrDefault(x => parameters is null || x.Key.SequenceEqual(parameters)).Value;
            if (matchConstructor is null)
            {
                throw new KeyNotFoundException($"{nameof(parameters)} '{parameters}' は無効なキーです。");
            }

            return matchConstructor;
        }

        public MethodInfo GetSourceMethodOf(string wrapperName, Type[] parameters = null)
        {
            MethodInfo matchMethod = Methods.FirstOrDefault(x => x.Key.Item1 == wrapperName && (parameters is null || x.Key.Item2.SequenceEqual(parameters))).Value;
            if (matchMethod is null)
            {
                throw new KeyNotFoundException($"{nameof(wrapperName)} '{wrapperName}' または {nameof(parameters)} '{parameters}' は無効なキーです。");
            }

            return matchMethod;
        }
    }
}
