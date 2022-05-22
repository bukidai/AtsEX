using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    /// <summary>
    /// 読み込まれた AtsEX プラグインの情報を表します。
    /// </summary>
    public class AtsExPluginInfo
    {
        /// <summary>
        /// AtsEX プラグインのソースの <see cref="SourceAssembly"/> を取得します。 
        /// </summary>
        public Assembly SourceAssembly { get; }

        /// <summary>
        /// AtsEX プラグインの <see cref="AtsExPluginBase"/> を継承するメインクラスの完全修飾名を取得します。
        /// </summary>
        public string MainClassFullName { get; }

        /// <summary>
        /// AtsEX プラグインの <see cref="AtsExPluginBase"/> を継承するメインクラスのインスタンスを取得します。
        /// </summary>
        public AtsExPluginBase PluginInstance { get; }

        public AtsExPluginInfo(Assembly sourceAssembly, string mainClassFullName, AtsExPluginBase pluginInstance)
        {
            SourceAssembly = sourceAssembly;
            MainClassFullName = mainClassFullName;
            PluginInstance = pluginInstance;
        }
    }
}
