using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BveTypes
{
    internal abstract class ClassInitializerBase
    {
        public ClassInitializerBase()
        {
        }

        public abstract void InitializeAll();

        protected void Initialize<TTargetAttribute>(IEnumerable<Type> targetTypes, Func<MethodInfo, bool> methodSelector, object[] parameters) where TTargetAttribute : Attribute
        {
            Parallel.ForEach(targetTypes, type =>
            {
                MethodInfo initializeMethod = type
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Static)
                    .FirstOrDefault(method =>
                    {
                        if (method.GetCustomAttribute<TTargetAttribute>() is null) return false;
                        return methodSelector(method);
                    });

                initializeMethod?.Invoke(null, parameters);
            });
        }
    }
}
