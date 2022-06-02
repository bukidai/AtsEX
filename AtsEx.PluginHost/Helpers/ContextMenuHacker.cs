using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost.Helpers
{
    /// <summary>
    /// メインフォームの右クリックメニューを編集するための機能を提供します。
    /// </summary>
    public class ContextMenuHacker : IDisposable
    {
        private static IApp App;
        private static IBveHacker BveHacker;

        protected static ToolStripItemCollection ContextMenuItems;
        protected static int AddTargetIndex;

        protected List<ToolStripItem> AddedItems = new List<ToolStripItem>();

        [InitializeHelper]
        private static void Initialize(IApp app, IBveHacker bveHacker)
        {
            App = app;
            BveHacker = bveHacker;

            ContextMenuItems = BveHacker.MainForm.ContextMenu.Items;
            AddTargetIndex = ContextMenuItems.IndexOfKey("toolStripMenuItem") + 1;
        }

        public ContextMenuHacker()
        {
        }

        /// <summary>
        /// メニューに新しい項目を追加します。
        /// </summary>
        /// <param name="item">追加する項目。</param>
        /// <returns>追加した項目。</returns>
        public ToolStripItem AddItem(ToolStripItem item)
        {
            ContextMenuItems.Insert(AddTargetIndex, item);
            AddedItems.Add(item);

            AddTargetIndex++;

            return item;
        }

        /// <summary>
        /// メニューに新しいクリック可能な <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text">メニュー項目に表示するテキスト。</param>
        /// <param name="click">メニュー項目がクリックされたときに発生する <see cref="EventHandler"/>。</param>
        /// <returns>追加した項目。</returns>
        public ToolStripMenuItem AddClickableMenuItem(string text, EventHandler click)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            item.Click += click;
            AddMenuItem(item);

            return item;
        }

        /// <summary>
        /// メニューに新しいチェック可能な <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text">メニュー項目に表示するテキスト。</param>
        /// <param name="checkedChanged"><see cref="ToolStripMenuItem.Checked"/> プロパティの値が変化したときに発生する <see cref="EventHandler"/>。</param>
        /// <returns>追加した項目。</returns>
        public ToolStripMenuItem AddCheckableMenuItem(string text, EventHandler checkedChanged)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text)
            {
                CheckOnClick = true,
            };
            item.CheckedChanged += checkedChanged;
            AddMenuItem(item);

            return item;
        }

        /// <summary>
        /// メニューに新しいチェック可能な <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text">メニュー項目に表示するテキスト。</param>
        /// <returns>追加した項目。</returns>
        public ToolStripMenuItem AddCheckableMenuItem(string text) => AddCheckableMenuItem(text, (_, _1) => { });

        public ToolStripMenuItem AddMenuItem(ToolStripMenuItem item)
        {
            AddItem(item);
            return item;
        }

        public ToolStripSeparator AddSeparator()
        {
            ToolStripSeparator item = new ToolStripSeparator();
            AddItem(item);

            return item;
        }

        public void Dispose()
        {
            AddedItems.ForEach(item => ContextMenuItems.Remove(item));
            AddedItems.Clear();
        }
    }
}
