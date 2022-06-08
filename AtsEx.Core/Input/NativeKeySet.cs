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
    public class NativeKeySet : INativeKeySet
    {
        public ReadOnlyDictionary<NativeAtsKeyName, KeyBase> AtsKeys { get; }

        /// <summary>
        /// <see cref="NativeKeySet"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public NativeKeySet()
        {
            {
                NativeAtsKeyName[] allKeyNames = Enum.GetValues(typeof(NativeAtsKeyName)) as NativeAtsKeyName[];
                Dictionary<NativeAtsKeyName, KeyBase> keys = allKeyNames.ToDictionary(keyName => keyName, _ => new NativeAtsKey() as KeyBase);

                AtsKeys = new ReadOnlyDictionary<NativeAtsKeyName, KeyBase>(keys);
            }
        }
    }
}
