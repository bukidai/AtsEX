using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Input;
using Automatic9045.AtsEx.PluginHost.Input.Native;

namespace Automatic9045.AtsEx.Input
{
    internal class NativeKeySet : INativeKeySet
    {
        public ReadOnlyDictionary<NativeAtsKeyName, KeyBase> AtsKeys { get; }

        public NativeKeySet()
        {
            {
                NativeAtsKeyName[] allKeyNames = Enum.GetValues(typeof(NativeAtsKeyName)) as NativeAtsKeyName[];
                Dictionary<NativeAtsKeyName, KeyBase> keyDictionary = allKeyNames.ToDictionary(keyName => keyName, _ => new NativeAtsKey() as KeyBase);
                SortedList<NativeAtsKeyName, KeyBase> sortedKeyList = new SortedList<NativeAtsKeyName, KeyBase>(keyDictionary);

                AtsKeys = new ReadOnlyDictionary<NativeAtsKeyName, KeyBase>(sortedKeyList);
            }
        }
    }
}
