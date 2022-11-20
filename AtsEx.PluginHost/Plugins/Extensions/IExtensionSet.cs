using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Plugins.Extensions
{
    /// <summary>
    /// AtsEX 拡張機能の一覧を表します。
    /// </summary>
    public interface IExtensionSet : IEnumerable<PluginBase>
    {
        /// <summary>
        /// 指定した型の AtsEX 拡張機能を取得します。
        /// </summary>
        /// <typeparam name="TExtension">AtsEX 拡張機能の型。</typeparam>
        /// <returns><typeparamref name="TExtension"/> 型の AtsEX 拡張機能。</returns>
        TExtension GetExtension<TExtension>();
    }
}
