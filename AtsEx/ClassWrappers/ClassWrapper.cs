using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    public class ClassWrapper : IClassWrapper
    {
        public Type SrcType { get; }
        public dynamic Src { get; }

        public ClassWrapper(dynamic src)
        {
            Src = src;
            SrcType = Src.GetType();
        }

        [UnderConstruction]
        protected MethodInfo GetMethod(string methodName, params Type[] types)
        {
            Type[] actualTypes = types.Select(t =>
            {
                if (t.IsSubclassOf(typeof(ClassWrapper)))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    return t;
                }
            }).ToArray();

            MethodInfo method = SrcType.GetMethod(methodName, types);
            return method;
        }

        protected MethodInfo GetMethod(string methodName) => GetMethod(methodName, new Type[0]);

        [Obsolete]
        protected MethodInfo GetMethod(string methodName, Assembly assembly, params string[] typeNames)
        {
            Type[] types = typeNames.Select(n => assembly.GetType(n)).ToArray();
            return GetMethod(methodName, types);
        }
    }
}
