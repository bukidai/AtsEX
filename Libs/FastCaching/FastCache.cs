using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCaching
{
    public class FastCache<TKey, TValue> where TKey : class where TValue : class
    {
        private readonly object WriterLock = new object();
        private readonly Hashtable Items;

        public FastCache()
        {
            Items = new Hashtable();
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (Items.ContainsKey(key))
            {
                return Items[key] as TValue;
            }
            else
            {
                TValue value = valueFactory(key);
                lock (WriterLock) Items.Add(key, value);

                return value;
            }
        }
    }
}
