using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace FastMember
{
    public class FastField
    {
        private readonly Func<object, object> Getter;
        private readonly Action<object, object> Setter;

        public FieldInfo Source { get; }

        protected FastField(FieldInfo source, Func<object, object> getter, Action<object, object> setter)
        {
            Source = source;
            Getter = getter;
            Setter = setter;
        }

        public static FastField Create(FieldInfo source)
        {
            Func<object, object> getter = ReflectionExpressionGenerator.GenerateFieldGetter(source);
            Action<object, object> setter = ReflectionExpressionGenerator.GenerateFieldSetter(source);

            return new FastField(source, getter, setter);
        }

        public object GetValue(object instance) => Getter(instance);

        public void SetValue(object instance, object value) => Setter(instance, value);
    }
}
