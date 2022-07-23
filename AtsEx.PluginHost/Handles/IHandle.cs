using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Handles
{
    /// <summary>
    /// ブレーキなどのハンドルを表します。
    /// </summary>
    public interface IHandle
    {
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
        /// 指定した値にノッチを変更するコマンドを取得します。
        /// </summary>
        /// <param name="notch">変更先のノッチ。</param>
        /// <returns>ノッチを <paramref name="notch"/> で指定した値に変更する <see cref="NotchCommandBase"/>。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="notch"/> が <see cref="MinNotch"/> 未満か <see cref="MaxNotch"/> より大きいです。</exception>
        NotchCommandBase GetCommandToSetNotchTo(int notch);
    }
}
