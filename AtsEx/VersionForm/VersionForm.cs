using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx
{
    internal partial class VersionForm : Form
    {
        protected App App { get; }
        protected BveHacker BveHacker { get; }

        public VersionForm(App app, BveHacker bveHacker)
        {
            App = app;
            BveHacker = bveHacker;

            InitializeComponent();
        }

        public void SetPluginDetails(IEnumerable<AtsExPluginInfo> plugins)
        {
            ListViewItem[] listViewItems = plugins.Select(plugin =>
            {
                string fileName = Path.GetFileName(plugin.SourceAssembly.Location);
                string title = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(plugin.SourceAssembly, typeof(AssemblyTitleAttribute))).Title;
                string type = plugin.PluginType.GetTypeString();
                Color typeColor = plugin.PluginType.GetTypeColor();
                string version = plugin.SourceAssembly.GetName().Version.ToString();
                string description = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(plugin.SourceAssembly, typeof(AssemblyDescriptionAttribute))).Description;

                ListViewItem item = new ListViewItem();

                item.Text = fileName;
                item.ToolTipText = fileName;
                item.SubItems.Add(title);
                item.SubItems.Add(CreateColoredSubItem(type, typeColor));
                item.SubItems.Add(version);
                item.SubItems.Add(description);

                return item;
            }).ToArray();

            PluginList.Items.Clear();
            PluginList.Items.AddRange(listViewItems);


            ListViewItem.ListViewSubItem CreateColoredSubItem(string text, Color backgroundColor)
            {
                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem()
                {
                    Text = text,
                    BackColor = backgroundColor,
                };

                return subItem;
            }
        }
    }
}
