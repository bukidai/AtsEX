using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost
{
    public delegate void AllPluginLoadedEventHandler(AllPluginLoadedEventArgs e);
    public delegate void StartedEventHandler(StartedEventArgs e);
    public delegate void ElapseEventHandler(EventArgs e);

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
        /// <see cref="AtsExPlugin"/> のコンストラクタ内など、<see cref="AllVehiclePluginLoaded"/> イベントが発生するより前には取得できないので注意してください。
        /// </summary>
        List<AtsExPluginInfo> VehiclePlugins { get; }

        /// <summary>
        /// 読み込まれた AtsEX 路線プラグインのリストを取得します。
        /// <see cref="AtsExPlugin"/> のコンストラクタ内など、<see cref="AllMapPluginLoaded"/> イベントが発生するより前には取得できないので注意してください。
        /// </summary>
        List<AtsExPluginInfo> MapPlugins { get; }


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

        /// <summary>
        /// 毎フレーム発生します。従来の ATS プラグインの Elapse(ATS_VEHICLESTATE vehicleState, int[] panel, int[] sound) に当たります。
        /// </summary>
        event ElapseEventHandler Elapse;
    }
}
