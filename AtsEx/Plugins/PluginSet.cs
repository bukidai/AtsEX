using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.Plugins;

namespace AtsEx.Plugins
{
    internal class PluginSet : IPluginSet
    {
        protected readonly Dictionary<PluginType, ReadOnlyDictionary<string, PluginBase>> Items;

        public ReadOnlyDictionary<string, PluginBase> this[PluginType pluginType] => Items[pluginType];

        public PluginSet(IDictionary<string, PluginBase> vehiclePlugins, IDictionary<string, PluginBase> mapPlugins)
        {
            Items = new Dictionary<PluginType, ReadOnlyDictionary<string, PluginBase>>()
            {
                [PluginType.VehiclePlugin] = new ReadOnlyDictionary<string, PluginBase>(vehiclePlugins),
                [PluginType.MapPlugin] = new ReadOnlyDictionary<string, PluginBase>(mapPlugins),
            };
        }

        public IEnumerator<KeyValuePair<string, PluginBase>> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class Enumerator : IEnumerator<KeyValuePair<string, PluginBase>>
        {
            private readonly PluginSet Source;
            private readonly IEnumerator<PluginType> KeyEnumerator;

            private IEnumerator<KeyValuePair<string, PluginBase>> CurrentEnumerator = null;

            public KeyValuePair<string, PluginBase> Current => CurrentEnumerator.Current;
            object IEnumerator.Current => Current;

            public Enumerator(PluginSet source)
            {
                Source = source;
                KeyEnumerator = Source.Items.Keys.GetEnumerator();

                Reset();
            }

            public void Dispose()
            {
                KeyEnumerator.Dispose();
                CurrentEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                while (!CurrentEnumerator.MoveNext())
                {
                    bool isKeyOver = !KeyEnumerator.MoveNext();
                    if (isKeyOver) return false;

                    CurrentEnumerator.Dispose();
                    CurrentEnumerator = Source.Items[KeyEnumerator.Current].GetEnumerator();
                }

                return true;
            }

            public void Reset()
            {
                KeyEnumerator.Reset();
                _ = KeyEnumerator.MoveNext();

                CurrentEnumerator?.Dispose();
                CurrentEnumerator = Source.Items[KeyEnumerator.Current].GetEnumerator();
            }
        }
    }
}
