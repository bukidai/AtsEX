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
    /// <summary>
    /// メインフォームの右クリックメニューを編集するための機能を提供します。
    /// </summary>
    public interface IContextMenuHacker
    {
        /// <summary>
        /// メニューに新しい項目を追加します。
        /// </summary>
        /// <param name="item">追加する項目。</param>
        /// <returns>追加した項目。</returns>
        ToolStripItem AddItem(ToolStripItem item);

        /// <summary>
        /// メニューに新しいクリック可能な <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text">メニュー項目に表示するテキスト。</param>
        /// <param name="click">メニュー項目がクリックされたときに発生する <see cref="EventHandler"/>。</param>
        /// <returns>追加した項目。</returns>
        ToolStripMenuItem AddClickableMenuItem(string text, EventHandler click);

        /// <summary>
        /// メニューに新しいチェック可能な <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text">メニュー項目に表示するテキスト。</param>
        /// <param name="checkedChanged"><see cref="ToolStripMenuItem.Checked"/> プロパティの値が変化したときに発生する <see cref="EventHandler"/>。</param>
        /// <returns>追加した項目。</returns>
        ToolStripMenuItem AddCheckableMenuItem(string text, EventHandler checkedChanged);

        /// <summary>
        /// メニューに新しいチェック可能な <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text">メニュー項目に表示するテキスト。</param>
        /// <returns>追加した項目。</returns>
        ToolStripMenuItem AddCheckableMenuItem(string text);

        /// <summary>
        /// メニューに新しい <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="item">追加する項目。</param>
        /// <returns>追加した項目。</returns>
        ToolStripMenuItem AddMenuItem(ToolStripMenuItem item);

        /// <summary>
        /// メニューに新しい区切り線を表す <see cref="ToolStripSeparator"/> を追加します。
        /// </summary>
        /// <returns>追加した項目。</returns>
        ToolStripSeparator AddSeparator();
    }
}
