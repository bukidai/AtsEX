using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Plugins
{
    /// <summary>
    /// AtsEX プラグインの種類を指定します。
    /// </summary>
    public class PluginTypeAttribute : Attribute
    {
        /// <summary>
        /// AtsEX プラグインの種類を取得します。
        /// </summary>
        public PluginType PluginType { get; }

        /// <summary>
        /// <see cref="PluginTypeAttribute"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="pluginType">AtsEX プラグインの種類。</param>
        public PluginTypeAttribute(PluginType pluginType)
        {
            PluginType = pluginType;
        }
    }
}
