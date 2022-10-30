using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Plugins.Extensions
{
    /// <summary>
    /// AtsEX 拡張機能のファクトリの一覧を表します。
    /// </summary>
    public interface IExtensionFactorySet : IEnumerable<PluginBase>
    {
        /// <summary>
        /// 指定した型の AtsEX 拡張機能ファクトリを取得します。
        /// </summary>
        /// <typeparam name="TExtensionFactory">AtsEX 拡張機能ファクトリ の型。</typeparam>
        /// <returns><typeparamref name="TExtensionFactory"/> 型の AtsEX 拡張機能ファクトリ。</returns>
        PluginBase GetFactory<TExtensionFactory>() where TExtensionFactory : PluginBase;
    }
}
