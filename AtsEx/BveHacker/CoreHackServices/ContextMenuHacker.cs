using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.ClassWrappers;

namespace Automatic9045.AtsEx.CoreHackServices
{
    internal interface IContextMenuHacker : IDisposable
    {
        ToolStripItem AddItem(ToolStripItem item);
        ToolStripMenuItem AddClickableMenuItem(string text, EventHandler click);
        ToolStripMenuItem AddCheckableMenuItem(string text, EventHandler checkedChanged, bool checkByDefault);
        ToolStripMenuItem AddCheckableMenuItem(string text, bool checkByDefault);
        ToolStripMenuItem AddMenuItem(ToolStripMenuItem item);
        ToolStripSeparator AddSeparator();
    }

    internal sealed class ContextMenuHacker : CoreHackService, IContextMenuHacker
    {
        private ToolStripItemCollection ContextMenuItems;

        private int DefaultItemCount;
        private int AddStartIndex;
        private int AddedItemCount = 0;

        public ContextMenuHacker(Process targetProcess, Assembly targetAssembly, ServiceCollection services) : base(targetProcess, targetAssembly, services)
        {
            Form formSource = Services.GetService<IMainFormHacker>().TargetForm;
            MainForm MainForm = new MainForm(formSource);

            ContextMenuItems = MainForm.ContextMenu.Items;

            DefaultItemCount = ContextMenuItems.Count;
            AddStartIndex = ContextMenuItems.IndexOfKey("toolStripMenuItem") + 1;

            AddSeparator();
        }

        public ToolStripItem AddItem(ToolStripItem item)
        {
            if (ContextMenuItems.Count != DefaultItemCount + AddedItemCount)
            {
                throw new Exception("AtsEX 以外から右クリックメニューを編集しないでください。");
            }

            ContextMenuItems.Insert(AddStartIndex + AddedItemCount, item);
            AddedItemCount++;

            return item;
        }

        public ToolStripMenuItem AddClickableMenuItem(string text, EventHandler click)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            item.Click += click;
            AddMenuItem(item);

            return item;
        }

        public ToolStripMenuItem AddCheckableMenuItem(string text, EventHandler checkedChanged, bool checkByDefault)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text)
            {
                CheckOnClick = true,
                Checked = checkByDefault,
            };
            item.CheckedChanged += checkedChanged;
            AddMenuItem(item);

            return item;
        }

        public ToolStripMenuItem AddCheckableMenuItem(string text, bool checkByDefault) => AddCheckableMenuItem(text, (_, _1) => { }, checkByDefault);

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
            for (int i = AddedItemCount - 1; i >= 0; i--)
            {
                ContextMenuItems.RemoveAt(AddStartIndex + i);
            }
        }
    }
}
