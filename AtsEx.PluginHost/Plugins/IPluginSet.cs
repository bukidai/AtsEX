using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Plugins
{
    /// <summary>
    /// AtsEX プラグインの一覧を表します。
    /// </summary>
    public interface IPluginSet : IEnumerable<KeyValuePair<string, PluginBase>>
    {
        /// <summary>
        /// 指定した種類の AtsEX プラグインの一覧を取得します。
        /// </summary>
        ReadOnlyDictionary<string, PluginBase> this[PluginType pluginType] { get; }
    }
}
