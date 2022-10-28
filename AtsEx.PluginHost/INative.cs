using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.Handles;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Native;

namespace AtsEx.PluginHost
{
    /// <summary>
    /// BVE が標準で提供する ATS プラグイン向けの機能をラップします。
    /// </summary>
    public interface INative
    {
        /// <summary>
        /// 全てのハンドルのセットを取得します。
        /// </summary>
        /// <remarks>
        /// このプロパティに設定されている値は力行ハンドルの抑速ノッチ、ブレーキハンドルの抑速ブレーキノッチを無視したものになります。<br/>
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
        /// シナリオ開始時に発生します。従来の ATS プラグインの Initialize(int brake) に当たります。
        /// </summary>
        event StartedEventHandler Started;
    }
}
