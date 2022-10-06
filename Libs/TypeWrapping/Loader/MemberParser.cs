using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypeWrapping
{
    internal class MemberParser
    {
        private readonly Type Source;

        private readonly Dictionary<string, FieldInfo> FieldCache = new Dictionary<string, FieldInfo>();
        private readonly Dictionary<string, PropertyInfo> PropertyCache = new Dictionary<string, PropertyInfo>();
        private readonly Dictionary<(string Name, Type[] Types), MethodInfo> MethodCache = new Dictionary<(string Name, Type[] Types), MethodInfo>();

        public MemberParser(Type source)
        {
            Source = source;
        }

        public FieldInfo GetField(string name, bool isNonPublic, bool isStatic)
        {
            if (FieldCache.TryGetValue(name, out FieldInfo field)) return field;

            field = Source.GetField(name, CreateBindingAttribute(isNonPublic, isStatic));
            FieldCache.Add(name, field);
            return field;
        }

        public PropertyInfo GetProperty(string name, bool isNonPublic, bool isStatic)
        {
            if (PropertyCache.TryGetValue(name, out PropertyInfo property)) return property;

            property = Source.GetProperty(name, CreateBindingAttribute(isNonPublic, isStatic));
            PropertyCache.Add(name, property);
            return property;
        }

        public MethodInfo GetMethod(string name, Type[] types, bool isNonPublic, bool isStatic)
        {
            if (MethodCache.TryGetValue((name, types), out MethodInfo method)) return method;

            method = Source.GetMethod(name, CreateBindingAttribute(isNonPublic, isStatic), null, types, null);
            MethodCache.Add((name, types), method);
            return method;
        }

        private BindingFlags CreateBindingAttribute(bool isNonPublic, bool isStatic)
        {
            BindingFlags result = BindingFlags.Default;
            result |= isNonPublic ? BindingFlags.NonPublic | BindingFlags.InvokeMethod : BindingFlags.Public;
            result |= isStatic ? BindingFlags.Static : BindingFlags.Instance;
            return result;
        }
    }
}
