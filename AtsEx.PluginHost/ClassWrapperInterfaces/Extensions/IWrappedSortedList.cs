using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost
{
    public interface IWrappedSortedList<TKey, TValueWrapper> : IDictionary<TKey, TValueWrapper>, IDictionary, IReadOnlyDictionary<TKey, TValueWrapper>
    {
        int Capacity { get; set; }
        Comparer<TValueWrapper> Comparer { get; }

        bool ContainsValue(TValueWrapper value);
        int IndexOfKey(TKey key);
        int IndexOfValue(TValueWrapper value);
        void RemoveAt(int index);
        void TrimExcess();
    }
}
