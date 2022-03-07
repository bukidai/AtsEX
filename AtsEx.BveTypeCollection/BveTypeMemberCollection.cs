using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx.BveTypeCollection
{
    public class BveTypeMemberCollection : IBveTypeMemberCollection
    {
        public Type WrapperType { get; }
        public Type OriginalType { get; }

        protected SortedList<string, MethodInfo> PropertyGetters { get; }
        protected SortedList<string, MethodInfo> PropertySetters { get; }
        protected SortedList<string, FieldInfo> Fields { get; }
        protected SortedList<(string, Type[]), MethodInfo> Methods { get; }

        public BveTypeMemberCollection(Type wrapperType, Type originalType,
            SortedList<string, MethodInfo> propertyGetters, SortedList<string, MethodInfo> propertySetters, SortedList<string, FieldInfo> fields, SortedList<(string, Type[]), MethodInfo> methods)
        {
            WrapperType = wrapperType;
            OriginalType = originalType;

            PropertyGetters = propertyGetters;
            PropertySetters = propertySetters;
            Fields = fields;
            Methods = methods;

#if DEBUG
            {
                IEnumerable<string> allMethods = WrapperType.GetMethods().Where(m => !m.IsSpecialName).Select(m => m.Name);

                PropertyInfo[] allProperties = WrapperType.GetProperties();
                IEnumerable<string> allGettableProperties = allProperties.Where(p => !(p.GetMethod is null)).Select(p => p.Name);
                IEnumerable<string> allSettableProperties = allProperties.Where(p => !(p.SetMethod is null)).Select(p => p.Name);

                if (allGettableProperties.Any(p => !PropertyGetters.ContainsKey(p) && !Fields.ContainsKey(p))
                    || allSettableProperties.Any(p => !PropertySetters.ContainsKey(p) && !Fields.ContainsKey(p))
                    || allMethods.Any(m => !Methods.Keys.Any(x => x.Item1 == m)))
                {
                    throw new DevelopException($"{GetType().Name} にて定義されていない {WrapperType.Name} のメンバーがあります。");
                }
            }
#endif
        }

        public MethodInfo GetSourcePropertyGetterOf(string wrapperName)
        {
            if (!PropertyGetters.Keys.Contains(wrapperName))
            {
                throw new ArgumentException($"{nameof(wrapperName)} '{wrapperName}' は無効なキーです。");
            }

            return PropertyGetters[wrapperName];
        }

        public MethodInfo GetSourcePropertySetterOf(string wrapperName)
        {
            if (!PropertySetters.Keys.Contains(wrapperName))
            {
                throw new ArgumentException($"{nameof(wrapperName)} '{wrapperName}' は無効なキーです。");
            }

            return PropertySetters[wrapperName];
        }

        public FieldInfo GetSourceFieldOf(string wrapperName)
        {
            if (!Fields.Keys.Contains(wrapperName))
            {
                throw new ArgumentException($"{nameof(wrapperName)} '{wrapperName}' は無効なキーです。");
            }

            return Fields[wrapperName];
        }

        public MethodInfo GetSourceMethodOf(string wrapperName, Type[] parameters = null)
        {
            MethodInfo matchMethod = Methods.FirstOrDefault(x => x.Key.Item1 == wrapperName && (parameters is null || x.Key.Item2.SequenceEqual(parameters))).Value;
            if (matchMethod is null)
            {
                throw new ArgumentException($"{nameof(wrapperName)} '{wrapperName}' は無効なキーです。");
            }

            return matchMethod;
        }
    }
}
