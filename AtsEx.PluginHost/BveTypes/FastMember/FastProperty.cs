using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    public class FastProperty
    {
        private readonly Func<object, object> Getter;
        private readonly Action<object, object> Setter;

        public PropertyInfo Source { get; }

        protected FastProperty(PropertyInfo source, Func<object, object> getter, Action<object, object> setter)
        {
            Source = source;
            Getter = getter;
            Setter = setter;
        }

        public static FastProperty Create(PropertyInfo source)
        {
            Func<object, object> getter = ReflectionExpressionGenerator.GeneratePropertyGetter(source);
            Action<object, object> setter = ReflectionExpressionGenerator.GeneratePropertySetter(source);

            return new FastProperty(source, getter, setter);
        }

        public object GetValue(object instance) => Getter(instance);

        public void SetValue(object instance, object value) => Setter(instance, value);
    }
}
