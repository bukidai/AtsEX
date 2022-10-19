using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Plugins
{
    /// <summary>
    /// アセンブリ (.dll) 形式の AtsEX プラグインを表します。
    /// </summary>
    public abstract class AssemblyPluginBase : PluginBase
    {
        /// <inheritdoc/>
        public override string Location { get; }

        /// <inheritdoc/>
        public override string Name { get; }

        /// <inheritdoc/>
        public override string Title { get; }

        /// <inheritdoc/>
        public override string Version { get; }

        /// <inheritdoc/>
        public override string Description { get; }

        /// <inheritdoc/>
        public override string Copyright { get; }

        /// <summary>
        /// AtsEX 拡張機能を利用する AtsEX プラグインの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="builder">AtsEX から渡される BVE、AtsEX の情報。</param>
        /// <param name="pluginType">プラグインの種別。</param>
        public AssemblyPluginBase(PluginBuilder builder, PluginType pluginType) : this(builder, pluginType, true)
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
        public AssemblyPluginBase(PluginBuilder builder, PluginType pluginType, bool useAtsExExtensions) : base(builder, pluginType, useAtsExExtensions)
        {
            Assembly pluginAssembly = GetType().Assembly;

            Location = pluginAssembly.Location;
            Name = Path.GetFileName(Location);
            Title = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(pluginAssembly, typeof(AssemblyTitleAttribute))).Title;
            Version = pluginAssembly.GetName().Version.ToString();
            Description = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(pluginAssembly, typeof(AssemblyDescriptionAttribute))).Description;
            Copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(pluginAssembly, typeof(AssemblyCopyrightAttribute))).Copyright;
        }
    }
}
