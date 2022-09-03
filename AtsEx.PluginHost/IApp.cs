using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Input.Native;
using Automatic9045.AtsEx.PluginHost.Plugins;

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


        /// <summary>
        /// 読み込まれた AtsEX プラグインの一覧を取得します。
        /// </summary>
        /// <remarks>
        /// <see cref="PluginBase"/> のコンストラクタ内など、
        /// 車両プラグインは <see cref="AllVehiclePluginLoaded"/> イベント、マッププラグインは <see cref="AllMapPluginLoaded"/> イベントが発生するより前には取得できないので注意してください。
        /// </remarks>
        ReadOnlyDictionary<PluginType, ReadOnlyDictionary<string, PluginBase>> Plugins { get; }


        /// <summary>
        /// 全てのハンドルのセットを取得します。
        /// </summary>
        /// <remarks>
        /// <see cref="BveHacker"/> が利用できない場合、このプロパティに設定されている値は力行ハンドルの抑速ノッチ、ブレーキハンドルの抑速ブレーキノッチを無視したものになります。<br/>
        /// 正確な値を確実に取得したい場合は <see cref="BveHacker.Handles"/> プロパティを使用してください。
        /// </remarks>
        /// <seealso cref="BveHacker.Handles"/>
        HandleSet Handles { get; }


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
        /// 全ての AtsEX 車両プラグインの読込が完了し、車両プラグインの一覧が <see cref="Plugins"/> から取得可能になると発生します。
        /// マッププラグインは発生時点では読み込めないので注意してください。
        /// </summary>
        event AllPluginLoadedEventHandler AllVehiclePluginLoaded;

        /// <summary>
        /// 全ての AtsEX マッププラグインの読込が完了し、マッププラグインの一覧が <see cref="Plugins"/> から取得可能になると発生します。
        /// </summary>
        event AllPluginLoadedEventHandler AllMapPluginLoaded;


        /// <summary>
        /// シナリオ開始時に発生します。従来の ATS プラグインの Initialize(int brake) に当たります。
        /// </summary>
        event StartedEventHandler Started;
    }
}
