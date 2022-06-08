using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Input.Native
{
    /// <summary>
    /// BVE が ATS プラグイン向けに提供するキーの入力情報を表します。
    /// </summary>
    public interface INativeKeySet
    {
        /// <summary>
        /// ATS キーの入力情報を取得します。
        /// </summary>
        ReadOnlyDictionary<NativeAtsKeyName, KeyBase> AtsKeys { get; }
    }
}
