using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace FastMember
{
    public class FastConstructor
    {
        private readonly Func<object[], object> Invoker;

        public ConstructorInfo Source { get; }

        protected FastConstructor(ConstructorInfo source, Func<object[], object> invoker)
        {
            Source = source;
            Invoker = invoker;
        }

        public static FastConstructor Create(ConstructorInfo source)
        {
            Func<object[], object> invoker = ReflectionExpressionGenerator.GenerateConstructorInvoker(source);

            return new FastConstructor(source, invoker);
        }

        public object Invoke(object[] args) => Invoker(args);
    }
}
