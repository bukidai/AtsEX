using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using UnembeddedResources;

using AtsEx.PluginHost.Plugins.Extensions;

namespace AtsEx.PluginHost.Plugins
{
    /// <summary>
    /// AtsEX プラグインを表します。
    /// </summary>
    public abstract class PluginBase : IDisposable
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<PluginBase>("PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> CannotUseAtsExExtensions { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static PluginBase()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        /// <summary>
        /// この AtsEX プラグインの種類を取得します。
        /// </summary>
        public PluginType PluginType { get; }

        /// <summary>
        /// この AtsEX プラグインが AtsEX 拡張機能 (<see cref="PluginHost.BveHacker"/> など) を使用するかどうかを取得します。
        /// </summary>
        /// <remarks>
        /// <see langword="false"/> に設定されている場合、<see cref="BveHacker"/> プロパティは取得できません。
        /// </remarks>
        public bool UseAtsExExtensions { get; }

        /// <summary>
        /// BVE が標準で提供する ATS プラグイン向けの機能のラッパーを取得します。
        /// </summary>
        protected INative Native { get; }

        private IExtensionFactorySet _Extensions = null;
        /// <summary>
        /// 読み込まれた AtsEX 拡張機能ファクトリの一覧を取得します。
        /// </summary>
        /// <remarks>
        /// AtsEX 拡張機能の場合 (<see cref="PluginType"/> が <see cref="PluginType.Extension"/> の場合) は取得できません。
        /// </remarks>
        protected IExtensionFactorySet Extensions => _Extensions ?? throw new PropertyNotInitializedException(nameof(Extensions));

        private IPluginSet _Plugins = null;
        /// <summary>
        /// 読み込まれた AtsEX プラグインの一覧を取得します。
        /// </summary>
        /// <remarks>
        /// <para>
        /// AtsEX 拡張機能の場合 (<see cref="PluginType"/> が <see cref="PluginType.Extension"/> の場合) 、<see cref="AllExtensionsLoaded"/> イベントが発生するまでは取得できません。
        /// </para>
        /// <para>
        /// AtsEX 拡張機能ファクトリは <see cref="Extensions"/> プロパティから取得できます。
        /// </para>
        /// </remarks>
        protected IPluginSet Plugins => _Plugins ?? throw new PropertyNotInitializedException(nameof(Plugins));

        private readonly BveHacker _BveHacker = null;
        protected BveHacker BveHacker
            => UseAtsExExtensions ? _BveHacker : throw new InvalidOperationException(string.Format(Resources.Value.CannotUseAtsExExtensions.Value, nameof(UseAtsExExtensions)));

        /// <summary>
        /// 全ての AtsEX プラグインが読み込まれ、<see cref="Plugins"/> プロパティが取得可能になると発生します。
        /// </summary>
        /// <remarks>
        /// AtsEX 拡張機能の場合 (<see cref="PluginType"/> が <see cref="PluginType.Extension"/> の場合) は取得できません。
        /// </remarks>
        protected event AllPluginsLoadedEventHandler AllPluginsLoaded;

        /// <summary>
        /// 全ての AtsEX 拡張機能が読み込まれ、<see cref="Extensions"/> プロパティが取得可能になると発生します。
        /// </summary>
        /// <remarks>
        /// AtsEX 拡張機能以外の AtsEX プラグインの場合 (<see cref="PluginType"/> が <see cref="PluginType.Extension"/> 以外の場合)、<see cref="Extensions"/> プロパティは初めから取得可能です。
        /// </remarks>
        protected event AllExtensionsLoadedEventHandler AllExtensionsLoaded;

        /// <summary>
        /// PluginUsing ファイルで指定したこの  AtsEX プラグインの識別子を取得します。このプロパティの値は全プラグインにおいて一意であることが保証されています。
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// この AtsEX プラグインのファイルの完全パスを取得します。
        /// </summary>
        public abstract string Location { get; }

        /// <summary>
        /// この AtsEX プラグインのファイル名を取得します。
        /// </summary>
        /// <remarks>
        /// 通常はプラグイン パッケージ ファイルの名前と拡張子 (例: MyPlugin.dll) を表しますが、<br/>
        /// スクリプト プラグインの場合など、ファイル名でプラグインを判別できない場合 (例: Package.xml) は代替の文字列を使用することもできます。
        /// </remarks>
        public abstract string Name { get; }

        /// <summary>
        /// この AtsEX プラグインのタイトルを取得します。
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// この AtsEX プラグインのバージョンを表す文字列を取得します。
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// この AtsEX プラグインの説明を取得します。
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// この AtsEX プラグインの著作権表示を取得します。
        /// </summary>
        public abstract string Copyright { get; }

        /// <summary>
        /// AtsEX 拡張機能を利用する AtsEX プラグインの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="builder">AtsEX から渡される BVE、AtsEX の情報。</param>
        /// <param name="pluginType">プラグインの種類。</param>
        public PluginBase(PluginBuilder builder, PluginType pluginType) : this(builder, pluginType, true)
        {
        }

        /// <summary>
        /// AtsEX プラグインの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="builder">AtsEX から渡される BVE、AtsEX の情報。</param>
        /// <param name="pluginType">AtsEX プラグインの種類。</param>
        /// <param name="useAtsExExtensions">AtsEX 拡張機能を利用するか。<br/>
        /// <see langword="false"/> を指定すると、<see cref="BveHacker"/> が取得できなくなる代わりに、BVE のバージョンの問題で AtsEX 拡張機能の読込に失敗した場合でもシナリオを開始できるようになります。<br/>
        /// マッププラグインでは <see langword="false"/> を指定することはできません。</param>
        public PluginBase(PluginBuilder builder, PluginType pluginType, bool useAtsExExtensions)
        {
            PluginType = pluginType;
            UseAtsExExtensions = useAtsExExtensions;
            Native = builder.Native;
            _Extensions = builder.Extensions;
            Identifier = builder.Identifier;

            if (!UseAtsExExtensions) return;
            _BveHacker = builder.BveHacker;
            builder.AllExtensionsLoaded += e => _Extensions = e.Extensions;
            builder.AllExtensionsLoaded += e => AllExtensionsLoaded?.Invoke(e);

            builder.AllPluginsLoaded += e => _Plugins = e.Plugins;
            builder.AllPluginsLoaded += e => AllPluginsLoaded?.Invoke(e);

        }

        private void ABC(AllExtensionsLoadedEventArgs e) => MessageBox.Show("a");
        private void DEF(AllPluginsLoadedEventArgs e) => MessageBox.Show("ba");

        /// <inheritdoc/>
        public abstract void Dispose();

        /// <summary>
        /// 毎フレーム呼び出されます。従来の ATS プラグインの Elapse(ATS_VEHICLESTATE vehicleState, int[] panel, int[] sound) に当たります。
        /// </summary>
        /// <param name="elapsed">前フレームから経過した時間。</param>
        /// <returns>
        /// このメソッドの実行結果を表す <see cref="TickResult"/>。<br/>
        /// 拡張機能では <see cref="ExtensionTickResult"/> を、車両プラグインでは <see cref="VehiclePluginTickResult"/> を、マッププラグインでは <see cref="MapPluginTickResult"/> を返してください。
        /// </returns>
        public abstract TickResult Tick(TimeSpan elapsed);
    }
}
