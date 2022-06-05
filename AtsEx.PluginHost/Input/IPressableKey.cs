using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Input
{
    /// <summary>
    /// 押したり離したりすることができるキーを表します。
    /// </summary>
    public interface IPressableKey
    {
        /// <summary>
        /// キーが押されたことをこの <see cref="IPressableKey"/> オブジェクトに通知します。
        /// </summary>
        void Press();

        /// <summary>
        /// キーが離されたことをこの <see cref="IPressableKey"/> オブジェクトに通知します。
        /// </summary>
        void Release();
    }
}
