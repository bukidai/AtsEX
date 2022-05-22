using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost
{
    /// <summary>
    /// AtsEX プラグインを表します。
    /// </summary>
    public abstract class AtsExPluginBase
    {
        public PluginType PluginType { get; }

        protected IApp App { get; }
        protected IBveHacker BveHacker { get; }

        /// <summary>
        /// AtsEX 拡張機能を利用する AtsEX プラグインの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="services">AtsEX から渡される BVE、AtsEX の情報。</param>
        /// <param name="pluginType">プラグインの種別。</param>
        public AtsExPluginBase(HostServiceCollection services, PluginType pluginType)
        {
            PluginType = pluginType;

            App = services.App;
            BveHacker = services.BveHacker;
        }

        /// <summary>
        /// 毎フレーム呼び出されます。従来の ATS プラグインの Elapse(ATS_VEHICLESTATE vehicleState, int[] panel, int[] sound) に当たります。
        /// </summary>
        public abstract void Tick();
    }
}
