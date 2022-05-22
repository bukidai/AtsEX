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
        public bool UseAtsExExtensions { get; }
        protected IApp App { get; }

        private IBveHacker _BveHacker = null;
        protected IBveHacker BveHacker => UseAtsExExtensions ? _BveHacker : throw new InvalidOperationException($"{nameof(UseAtsExExtensions)} が {false} に設定されています。");

        /// <summary>
        /// AtsEX 拡張機能を利用する AtsEX プラグインの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="services">AtsEX から渡される BVE、AtsEX の情報。</param>
        /// <param name="pluginType">プラグインの種別。</param>
        public AtsExPluginBase(HostServiceCollection services, PluginType pluginType) : this(services, pluginType, true)
        {
        }

        /// <summary>
        /// AtsEX プラグインの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="services">AtsEX から渡される BVE、AtsEX の情報。</param>
        /// <param name="pluginType">AtsEX プラグインの種別。</param>
        /// <param name="useAtsExExtensions">AtsEX 拡張機能を利用するか。<br/>
        /// <see langword="false"/> を指定すると、<see cref="BveHacker"/> が取得できなくなる代わりに、BVE のバージョンの問題で AtsEX 拡張機能の読込に失敗した場合でもシナリオを開始できるようになります。<br/>
        /// マッププラグインでは <see langword="false"/> を指定することはできません。</param>
        public AtsExPluginBase(HostServiceCollection services, PluginType pluginType, bool useAtsExExtensions)
        {
            PluginType = pluginType;
            UseAtsExExtensions = useAtsExExtensions;
            App = services.App;

            if (!UseAtsExExtensions) return;

            _BveHacker = services.BveHacker;
        }

        /// <summary>
        /// 毎フレーム呼び出されます。従来の ATS プラグインの Elapse(ATS_VEHICLESTATE vehicleState, int[] panel, int[] sound) に当たります。
        /// </summary>
        public abstract void Tick();
    }
}
