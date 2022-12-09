using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Plugins
{
    /// <summary>
    /// AtsEX 独自の特殊機能拡張 (<see cref="IBveHacker"/>、マッププラグインなど) を利用しないプラグインであることを指定します。
    /// </summary>
    /// <remarks>
    /// この属性を付加した <see cref="PluginBase"/> では、<see cref="PluginBase.BveHacker"/> が取得できなくなる代わりに、
    /// BVE のバージョンの問題で AtsEX の特殊機能拡張の読込に失敗した場合でもシナリオを開始できるようになります。<br/>
    /// マッププラグインではこの属性を指定することはできません。
    /// </remarks>
    public class DoNotUseBveHackerAttribute : Attribute
    {
        /// <summary>
        /// <see cref="DoNotUseBveHackerAttribute"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public DoNotUseBveHackerAttribute()
        {
        }
    }
}
