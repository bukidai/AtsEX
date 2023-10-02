using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastMember
{
    public class FastEvent
    {
        public FastMethod AddAccessor { get; }
        public FastMethod RemoveAccessor { get; }

        public FastField DelegateField { get; }

        public FastEvent(MethodInfo addAccessor, MethodInfo removeAccessor, FieldInfo delegateField)
        {
            AddAccessor = FastMethod.Create(addAccessor);
            RemoveAccessor = FastMethod.Create(removeAccessor);

            DelegateField = FastField.Create(delegateField);
        }

        public void Add(object instance, object eventHandler) => AddAccessor.Invoke(instance, new object[] { eventHandler });
        public void Remove(object instance, object eventHandler) => RemoveAccessor.Invoke(instance, new object[] { eventHandler });

        public object Invoke(object instance, object[] parameters)
        {
            // TODO: 高速化
            Delegate @delegate = (Delegate)DelegateField.GetValue(instance);
            return @delegate?.Method.Invoke(@delegate.Target, parameters);
        }
    }
}
