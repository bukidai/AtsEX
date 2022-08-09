using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.Handles;

namespace Automatic9045.AtsEx.PluginHost.Plugins
{
    /// <summary>
    /// AtsEX プラグインを表します。
    /// </summary>
    public abstract class PluginBase
    {
        public PluginType PluginType { get; }
        public bool UseAtsExExtensions { get; }
        protected IApp App { get; }

        private BveHacker _BveHacker = null;
        protected BveHacker BveHacker => UseAtsExExtensions ? _BveHacker : throw new InvalidOperationException($"{nameof(UseAtsExExtensions)} が {false} に設定されています。");

        /// <summary>
        /// AtsEX プラグインのファイルの完全パスを取得します。
        /// </summary>
        public abstract string Location { get; }

        /// <summary>
        /// AtsEX プラグインのファイル名を取得します。
        /// </summary>
        /// <remarks>
        /// 通常はプラグイン パッケージ ファイルの名前と拡張子 (例: MyPlugin.dll) を表しますが、<br/>
        /// スクリプト プラグインの場合など、ファイル名でプラグインを判別できない場合 (例: Package.xml) は代替の文字列を使用することもできます。
        /// </remarks>
        public abstract string Name { get; }

        /// <summary>
        /// AtsEX プラグインのタイトルを取得します。
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// AtsEX プラグインのバージョンを表す文字列を取得します。
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// AtsEX プラグインの説明を取得します。
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// AtsEX プラグインの著作権表示を取得します。
        /// </summary>
        public abstract string Copyright { get; }

        /// <summary>
        /// AtsEX 拡張機能を利用する AtsEX プラグインの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="builder">AtsEX から渡される BVE、AtsEX の情報。</param>
        /// <param name="pluginType">プラグインの種別。</param>
        public PluginBase(PluginBuilder builder, PluginType pluginType) : this(builder, pluginType, true)
        {
        }

        /// <summary>
        /// AtsEX プラグインの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="builder">AtsEX から渡される BVE、AtsEX の情報。</param>
        /// <param name="pluginType">AtsEX プラグインの種別。</param>
        /// <param name="useAtsExExtensions">AtsEX 拡張機能を利用するか。<br/>
        /// <see langword="false"/> を指定すると、<see cref="BveHacker"/> が取得できなくなる代わりに、BVE のバージョンの問題で AtsEX 拡張機能の読込に失敗した場合でもシナリオを開始できるようになります。<br/>
        /// マッププラグインでは <see langword="false"/> を指定することはできません。</param>
        public PluginBase(PluginBuilder builder, PluginType pluginType, bool useAtsExExtensions)
        {
            PluginType = pluginType;
            UseAtsExExtensions = useAtsExExtensions;
            App = builder.App;

            if (!UseAtsExExtensions) return;

            _BveHacker = builder.BveHacker;
        }

        /// <summary>
        /// 毎フレーム呼び出されます。従来の ATS プラグインの Elapse(ATS_VEHICLESTATE vehicleState, int[] panel, int[] sound) に当たります。
        /// </summary>
        /// <param name="elapsed">前フレームから経過した時間。</param>
        /// <returns>
        /// このメソッドの実行結果を表す <see cref="TickResult"/>。<br/>
        /// 車両プラグインでは <see cref="VehiclePluginTickResult"/> を、マッププラグインでは <see cref="MapPluginTickResult"/> を返してください。
        /// </returns>
        public abstract TickResult Tick(TimeSpan elapsed);
    }
}
