using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.Panels.Native;

namespace AtsEx.Panels
{
    internal class AtsPanelValue<T> : IAtsPanelValue<T>
    {
        private readonly Converter<T, int> ValueSerializer;
        private readonly Action Disposer;

        private T _Value;
        public T Value
        {
            get => _Value;
            set
            {
                if (value.Equals(Value)) return;

                _Value = value;
                SerializedValue = ValueSerializer(value);

                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int SerializedValue { get; private set; }

        public event EventHandler ValueChanged;

        public AtsPanelValue(T initialValue, Converter<T, int> valueSerializer, Action disposer)
        {
            ValueSerializer = valueSerializer;
            Disposer = disposer;

            Value = initialValue;
        }

        public void Dispose() => Disposer();
    }
}
