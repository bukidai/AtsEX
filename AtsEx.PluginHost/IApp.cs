using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.Plugins;

using Automatic9045.AtsEx.PluginHost.Input.Native;

namespace Automatic9045.AtsEx.PluginHost
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
        /// 読み込まれた AtsEX 車両プラグインのリストを取得します。
        /// <see cref="PluginBase"/> のコンストラクタ内など、<see cref="AllVehiclePluginLoaded"/> イベントが発生するより前には取得できないので注意してください。
        /// </summary>
        List<PluginInfo> VehiclePlugins { get; }

        /// <summary>
        /// 読み込まれた AtsEX 路線プラグインのリストを取得します。
        /// <see cref="PluginBase"/> のコンストラクタ内など、<see cref="AllMapPluginLoaded"/> イベントが発生するより前には取得できないので注意してください。
        /// </summary>
        List<PluginInfo> MapPlugins { get; }


        /// <summary>
        /// BVE が ATS プラグイン向けに提供するキーの入力情報を取得します。
        /// </summary>
        INativeKeySet NativeKeys { get; }


        /// <summary>
        /// BVE が ATS プラグイン向けに提供する車両の性能に関する情報を取得します。
        /// </summary>
        VehicleSpec VehicleSpec { get; }

        /// <summary>
        /// BVE が ATS プラグイン向けに提供する車両の状態に関する情報を取得します。
        /// このプロパティの値はフレーム毎に更新されます。
        /// </summary>
        VehicleState VehicleState { get; }


        /// <summary>
        /// 全ての AtsEX 車両プラグインの読込が完了し、<see cref="VehiclePlugins"/> が取得可能になると発生します。
        /// <see cref="MapPlugins"/> は発生時点では読み込めないので注意してください。
        /// </summary>
        event AllPluginLoadedEventHandler AllVehiclePluginLoaded;

        /// <summary>
        /// 全ての AtsEX 路線プラグインの読込が完了し、<see cref="MapPlugins"/> が取得可能になると発生します。
        /// </summary>
        event AllPluginLoadedEventHandler AllMapPluginLoaded;


        /// <summary>
        /// シナリオ開始時に発生します。従来の ATS プラグインの Initialize(int brake) に当たります。
        /// </summary>
        event StartedEventHandler Started;
    }
}
