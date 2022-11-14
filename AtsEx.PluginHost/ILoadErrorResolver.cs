using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

namespace AtsEx.PluginHost
{
    /// <summary>
    /// 読込時に発生した例外を解決するための機能を提供します。
    /// </summary>
    public interface ILoadErrorResolver
    {
        /// <summary>
        /// 例外を解決します。
        /// </summary>
        /// <param name="exception">解決する例外。</param>
        void Resolve(Exception exception);
    }
}
