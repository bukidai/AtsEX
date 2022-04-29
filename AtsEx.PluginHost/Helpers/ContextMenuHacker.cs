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
    public static class ContextMenuHacker
    {
        private static ToolStripItemCollection ContextMenuItems;

        private static int DefaultItemCount;
        private static int AddStartIndex;
        private static int AddedItemCount = 0;

        static ContextMenuHacker()
        {
            InstanceStore.Closing += Dispose;

            ContextMenuItems = InstanceStore.BveHacker.MainForm.ContextMenu.Items;

            DefaultItemCount = ContextMenuItems.Count;
            AddStartIndex = ContextMenuItems.IndexOfKey("toolStripMenuItem") + 1;

            AddSeparator();
        }

        public static ToolStripItem AddItem(ToolStripItem item)
        {
            if (ContextMenuItems.Count != DefaultItemCount + AddedItemCount)
            {
                throw new Exception("AtsEX 以外から右クリックメニューを編集しないでください。");
            }

            ContextMenuItems.Insert(AddStartIndex + AddedItemCount, item);
            AddedItemCount++;

            return item;
        }

        public static ToolStripMenuItem AddClickableMenuItem(string text, EventHandler click)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            item.Click += click;
            AddMenuItem(item);

            return item;
        }

        public static ToolStripMenuItem AddCheckableMenuItem(string text, EventHandler checkedChanged)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text)
            {
                CheckOnClick = true,
            };
            item.CheckedChanged += checkedChanged;
            AddMenuItem(item);

            return item;
        }

        public static ToolStripMenuItem AddCheckableMenuItem(string text) => AddCheckableMenuItem(text, (_, _1) => { });

        public static ToolStripMenuItem AddMenuItem(ToolStripMenuItem item)
        {
            AddItem(item);
            return item;
        }

        public static ToolStripSeparator AddSeparator()
        {
            ToolStripSeparator item = new ToolStripSeparator();
            AddItem(item);

            return item;
        }

        private static void Dispose(EventArgs e)
        {
            for (int i = AddedItemCount - 1; i >= 0; i--)
            {
                ContextMenuItems.RemoveAt(AddStartIndex + i);
            }
        }
    }
}
