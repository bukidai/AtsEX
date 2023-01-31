using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.Native;
using AtsEx.PluginHost;
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

        public void Tick(AtsIoArray source)
        {
            foreach (KeyValuePair<int, IAtsPanelValueWithChangeLog> x in RegisteredValues)
            {
                if (OldValues.TryGetValue(x.Key, out int oldValue) && source[x.Key] != oldValue)
                {
                    string senderName = $"ats{x.Key}";
                    throw new ConflictException(string.Format(Resources.Value.ChangeConflicted.Value, senderName), senderName);
                }

                if (!x.Value.IsChanged) continue;

                source[x.Key] = x.Value.SerializedValue;
                OldValues[x.Key] = x.Value.SerializedValue;

                x.Value.ApplyChanges();
            }
        }

        public IAtsPanelValue<TValue> Register<TValue>(int index, Converter<TValue, int> valueSerializer, TValue initialValue)
        {
            if (index < MinIndex || MaxIndex < index) throw new IndexOutOfRangeException();

            AtsPanelValue<TValue> value = new AtsPanelValue<TValue>(initialValue, valueSerializer, () => RegisteredValues.Remove(index));
            AtsPanelValueWithChangeLog<TValue> valueWithChangeLog = new AtsPanelValueWithChangeLog<TValue>(value);

            RegisteredValues.Add(index, valueWithChangeLog);
            return value;
        }

        public IAtsPanelValue<bool> RegisterBoolean(int index, bool initialValue) => Register(index, x => x ? 1 : 0, initialValue);
        public IAtsPanelValue<int> RegisterInt32(int index, int initialValue) => Register(index, x => x, initialValue);


        private sealed class AtsPanelValueWithChangeLog<T> : IAtsPanelValueWithChangeLog
        {
            public IAtsPanelValue<T> Value { get; }
            public int SerializedValue => Value.SerializedValue;
            public bool IsChanged { get; private set; } = true;

            public AtsPanelValueWithChangeLog(IAtsPanelValue<T> value)
            {
                Value = value;
                Value.ValueChanged += (sender, e) => IsChanged = true;
            }

            public void ApplyChanges() => IsChanged = false;
        }

        public interface IAtsPanelValueWithChangeLog
        {
            int SerializedValue { get; }
            bool IsChanged { get; }

            void ApplyChanges();
        }
    }
}
