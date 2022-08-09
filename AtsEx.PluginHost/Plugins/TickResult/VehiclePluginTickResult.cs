using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Handles;

namespace Automatic9045.AtsEx.PluginHost.Plugins
{
    /// <summary>
    /// 車両プラグインの <see cref="PluginBase.Tick(TimeSpan)"/> メソッドの実行結果を表します。
    /// </summary>
    public class VehiclePluginTickResult : TickResult
    {
        /// <summary>
        /// ハンドルの出力を編集するためのコマンドのセットを取得します。
        /// </summary>
        public HandleCommandSet HandleCommandSet { get; }

        /// <summary>
        /// <see cref="VehiclePluginTickResult"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="handleCommandSet">ハンドルの出力を編集するためのコマンドのセット。何もしないことを表すには <see cref="HandleCommandSet.DoNothing"/> を指定してください。</param>
        public VehiclePluginTickResult(HandleCommandSet handleCommandSet)
        {
            HandleCommandSet = handleCommandSet ?? throw new ArgumentNullException(nameof(handleCommandSet));
        }
    }
}
