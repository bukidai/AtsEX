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
    public class ContextMenuHacker
    {
        private static ContextMenuHacker instance = null;
        public static ContextMenuHacker Instance => instance = instance ?? new ContextMenuHacker();


        private ToolStripItemCollection ContextMenuItems;

        private int AddStartIndex;
        private List<ToolStripItem> AddedItems = new List<ToolStripItem>();

        private ContextMenuHacker()
        {
            InstanceStore.Closing += Dispose;

            ContextMenuItems = InstanceStore.BveHacker.MainForm.ContextMenu.Items;
            AddStartIndex = ContextMenuItems.IndexOfKey("toolStripMenuItem") + 1;

            AddSeparator();
        }

        public ToolStripItem AddItem(ToolStripItem item)
        {
            ContextMenuItems.Insert(AddStartIndex + AddedItems.Count, item);
            AddedItems.Add(item);

            return item;
        }

        public ToolStripMenuItem AddClickableMenuItem(string text, EventHandler click)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            item.Click += click;
            AddMenuItem(item);

            return item;
        }

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

        private void Dispose(EventArgs e)
        {
            AddedItems.ForEach(item => ContextMenuItems.Remove(item));
            AddedItems.Clear();

            InstanceStore.Closing -= Dispose;
            instance = null;
        }
    }
}
