using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost;

namespace AtsEx
{
    internal sealed class ContextMenuHacker : IContextMenuHacker, IDisposable
    {
        private readonly ToolStripItemCollection ContextMenuItems;
        private int AddTargetIndex;

        private readonly List<ToolStripItem> AddedPluginItems = new List<ToolStripItem>();
        private readonly List<ToolStripItem> AddedAtsExItems = new List<ToolStripItem>();

        public ContextMenuHacker(MainForm mainForm)
        {
            ContextMenuItems = mainForm.ContextMenu.Items;
            AddTargetIndex = ContextMenuItems.IndexOfKey("toolStripMenuItem") + 1;
        }

        public ToolStripItem AddItem(ToolStripItem item, bool isByAtsEx)
        {
            ContextMenuItems.Insert(AddTargetIndex, item);
            if (isByAtsEx)
            {
                AddedAtsExItems.Add(item);
            }
            else
            {
                AddedPluginItems.Add(item);
            }

            AddTargetIndex++;

            return item;
        }

        public ToolStripItem AddItem(ToolStripItem item) => AddItem(item, false);

        public ToolStripMenuItem AddClickableMenuItem(string text, EventHandler click)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            item.Click += click;
            AddMenuItem(item);

            return item;
        }

        public ToolStripMenuItem AddClickableMenuItem(string text, EventHandler click, bool isByAtsEx) => AddClickableMenuItem(text, click);

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
            AddItem(item, false);
            return item;
        }

        public ToolStripSeparator AddSeparator(bool isByAtsEx)
        {
            ToolStripSeparator item = new ToolStripSeparator();
            AddItem(item, isByAtsEx);

            return item;
        }

        public ToolStripSeparator AddSeparator() => AddSeparator(false);

        public void ClearPluginItems()
        {
            AddTargetIndex -= AddedPluginItems.Count;

            AddedPluginItems.ForEach(item => ContextMenuItems.Remove(item));
            AddedPluginItems.Clear();
        }

        public void Dispose()
        {
            ClearPluginItems();

            AddedAtsExItems.ForEach(item => ContextMenuItems.Remove(item));
            AddedAtsExItems.Clear();
        }
    }
}
