using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    public class FastMethod
    {
        private readonly Func<object, object[], object> Invoker;

        public MethodInfo Source { get; }

        protected FastMethod(MethodInfo source, Func<object, object[], object> invoker)
        {
            Source = source;
            Invoker = invoker;
        }

        public static FastMethod Create(MethodInfo source)
        {
            Func<object, object[], object> invoker = ReflectionExpressionGenerator.GenerateMethodInvoker(source);

            return new FastMethod(source, invoker);
        }

        public object Invoke(object instance, object[] args) => Invoker(instance, args);
    }
}
