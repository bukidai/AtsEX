using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.Plugins;

namespace AtsEx.PluginHost
{
    public delegate void AllPluginLoadedEventHandler(AllPluginLoadedEventArgs e);
    public delegate void StartedEventHandler(StartedEventArgs e);

    public interface IApp
    {
        /// <summary>
        /// プロダクト名を取得します。
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// 短いプロダクト名を取得します。
        /// </summary>
        string ProductShortName { get; }


        /// <summary>
        /// 制御対象の BVE を実行している <see cref="System.Diagnostics.Process"/> を取得します。
        /// </summary>
        Process Process { get; }

        /// <summary>
        /// AtsEX の <see cref="Assembly"/> を取得します。
        /// </summary>
        Assembly AtsExAssembly { get; }

        /// <summary>
        /// AtsEX プラグインホストの <see cref="Assembly"/> を取得します。
        /// </summary>
        Assembly AtsExPluginHostAssembly { get; }

        /// <summary>
        /// 実行元の BVE の <see cref="Assembly"/> を取得します。
        /// </summary>
        Assembly BveAssembly { get; }

        /// <summary>
        /// 実行元の BVE のバージョンを取得します。
        /// </summary>
        Version BveVersion { get; }
    }
}
