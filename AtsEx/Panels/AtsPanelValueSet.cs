using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.Native.Ats;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Binding;
using AtsEx.PluginHost.Panels.Native;

namespace AtsEx.Panels
{
    internal sealed class AtsPanelValueSet : IAtsPanelValueSet
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<AtsPanelValueSet>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ChangeConflicted { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private const int MinIndex = 0;
        private const int MaxIndex = 255;

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        private static readonly TwoWayConverter<bool, int> BooleanSerializer = new TwoWayConverter<bool, int>(x => x ? 1 : 0, x => x != 0);
        private static readonly TwoWayConverter<int, int> Int32Serializer = new TwoWayConverter<int, int>(x => x, x => x);

        static AtsPanelValueSet()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private readonly Dictionary<int, IAtsPanelValueWithChangeLog> RegisteredValues = new Dictionary<int, IAtsPanelValueWithChangeLog>();
        private readonly Dictionary<int, int> OldValues = new Dictionary<int, int>();

        public AtsPanelValueSet()
        {
        }

        public void PreTick(IList<int> source)
        {
            foreach (KeyValuePair<int, IAtsPanelValueWithChangeLog> x in RegisteredValues)
            {
                if (OldValues.TryGetValue(x.Key, out int oldValue) && source[x.Key] != oldValue)
                {
                    if (x.Value.Mode == BindingMode.OneWay)
                    {
                        string senderName = $"ats{x.Key}";
                        throw new ConflictException(string.Format(Resources.Value.ChangeConflicted.Value, senderName), senderName);
                    }

                    x.Value.SerializedValue = source[x.Key];
                }
            }
        }

        public void Tick(IList<int> source)
        {
            foreach (KeyValuePair<int, IAtsPanelValueWithChangeLog> x in RegisteredValues)
            {
                if (!x.Value.IsChanged || x.Value.Mode == BindingMode.OneWayToSource) continue;

                source[x.Key] = x.Value.SerializedValue;
                OldValues[x.Key] = x.Value.SerializedValue;

                x.Value.ApplyChanges();
            }
        }

        public IAtsPanelValue<TValue> Register<TValue>(int index, ITwoWayConverter<TValue, int> valueSerializer, TValue initialValue, BindingMode mode)
        {
            if (index < MinIndex || MaxIndex < index) throw new IndexOutOfRangeException();

            AtsPanelValue<TValue> value = new AtsPanelValue<TValue>(initialValue, valueSerializer, () => RegisteredValues.Remove(index), mode);
            AtsPanelValueWithChangeLog<TValue> valueWithChangeLog = new AtsPanelValueWithChangeLog<TValue>(value);

            RegisteredValues.Add(index, valueWithChangeLog);
            return value;
        }

        public IAtsPanelValue<TValue> Register<TValue>(int index, Converter<TValue, int> oneWayValueSerializer, TValue initialValue)
            => Register(index, new TwoWayConverter<TValue, int>(x => oneWayValueSerializer(x), null), initialValue, BindingMode.OneWay);

        public IAtsPanelValue<bool> RegisterBoolean(int index, bool initialValue, BindingMode mode) => Register(index, BooleanSerializer, initialValue, mode);
        public IAtsPanelValue<int> RegisterInt32(int index, int initialValue, BindingMode mode) => Register(index, Int32Serializer, initialValue, mode);


        private class TwoWayConverter<T1, T2> : ITwoWayConverter<T1, T2>
        {
            private readonly Func<T1, T2> ConvertFunc;
            private readonly Func<T2, T1> ConvertBackFunc;

            public TwoWayConverter(Func<T1, T2> convertFunc, Func<T2, T1> convertBackFunc)
            {
                ConvertFunc = convertFunc;
                ConvertBackFunc = convertBackFunc;
            }

            public T2 Convert(T1 value) => ConvertFunc(value);
            public T1 ConvertBack(T2 value) => ConvertBackFunc(value);
        }


        private sealed class AtsPanelValueWithChangeLog<T> : IAtsPanelValueWithChangeLog
        {
            public AtsPanelValue<T> Value { get; }
            public int SerializedValue
            {
                get => Value.SerializedValue;
                set => Value.SetValueExternally(value);
            }
            public BindingMode Mode => Value.Mode;
            public bool IsChanged { get; private set; } = true;

            public AtsPanelValueWithChangeLog(AtsPanelValue<T> value)
            {
                Value = value;
                Value.ValueChanged += (sender, e) => IsChanged = true;
            }

            public void ApplyChanges() => IsChanged = false;
        }

        public interface IAtsPanelValueWithChangeLog
        {
            int SerializedValue { get; set; }
            BindingMode Mode { get; }
            bool IsChanged { get; }

            void ApplyChanges();
        }
    }
}
