using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    public static class DictionaryExtensions
    {
        public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> source, TValue value, out TKey key, IEqualityComparer<TValue> comparer)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (comparer is null) throw new ArgumentNullException(nameof(comparer));

            foreach (KeyValuePair<TKey, TValue> item in source)
            {
                if (comparer.Equals(item.Value, value))
                {
                    key = item.Key;
                    return true;
                }
            }

            key = default;
            return false;
        }

        public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> source, TValue value, out TKey key) => TryGetKey(source, value, out key, EqualityComparer<TValue>.Default);

        public static TKey TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> source, TValue value, IEqualityComparer<TValue> comparer)
            => TryGetKey(source, value, out TKey result, comparer) ? result : default;

        public static TKey TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> source, TValue value) => TryGetKey(source, value, EqualityComparer<TValue>.Default);

        public static bool ContainsValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TValue value, IEqualityComparer<TValue> comparer)
            => TryGetKey(source, value, out TKey _, comparer);

        public static bool ContainsValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TValue value) => ContainsValue(source, value, EqualityComparer<TValue>.Default);
    }
}
