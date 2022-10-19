using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Handles
{
    /// <summary>
    /// ブレーキなどのハンドルを表します。
    /// </summary>
    public interface IHandle
    {
        /// <summary>
        /// <see cref="MaxNotch"/> を超えたノッチを設定することが許可されているかを取得します。
        /// </summary>
        bool CanSetNotchOutOfRange { get; }

        /// <summary>
        /// 最小のノッチを取得します。
        /// </summary>
        int MinNotch { get; }

        /// <summary>
        /// 最大のノッチを取得します。
        /// </summary>
        int MaxNotch { get; }

        /// <summary>
        /// 現在のノッチを取得します。
        /// </summary>
        int Notch { get; }

        /// <summary>
        /// <see cref="MaxNotch"/> を超えたノッチを設定することを許可します。
        /// </summary>
        /// <remarks>
        /// TASC プラグインなどを想定しています。<br/>
        /// AtsEX 以外から読み込まれたプラグインが <see cref="MaxNotch"/> を超えたノッチを設定する場合は、
        /// 必ず初めて <see cref="Plugins.PluginBase.Tick(TimeSpan)"/> メソッドが実行されるより前に実行してください。
        /// </remarks>
        void AllowSetNotchOutOfRange();

        /// <summary>
        /// 指定した値にノッチを変更するコマンドを取得します。
        /// </summary>
        /// <param name="notch">変更先のノッチ。</param>
        /// <returns>ノッチを <paramref name="notch"/> で指定した値に変更する <see cref="NotchCommandBase"/>。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="notch"/> が <see cref="MinNotch"/> 未満か <see cref="MaxNotch"/> より大きいです。</exception>
        NotchCommandBase GetCommandToSetNotchTo(int notch);
    }
}
