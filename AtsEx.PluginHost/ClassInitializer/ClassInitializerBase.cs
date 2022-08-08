using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    internal abstract class ClassInitializerBase
    {
        protected readonly IApp App;
        protected readonly BveHacker BveHacker;

        protected readonly object[] Parameters;

        public ClassInitializerBase(IApp app, BveHacker bveHacker)
        {
            App = app;
            BveHacker = bveHacker;

            Parameters = new object[] { App, BveHacker };
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
