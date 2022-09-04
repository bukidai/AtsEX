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
using Automatic9045.AtsEx.PluginHost.Plugins;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx
{
    internal partial class VersionForm : Form
    {
        protected static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<VersionForm>("Core");

        public VersionForm()
        {
            InitializeComponent();
        }

        public void SetPluginDetails(IEnumerable<PluginBase> plugins)
        {
            ListViewItem[] listViewItems = plugins.Select(plugin =>
            {
                Assembly pluginAssembly = plugin.GetType().Assembly;

                Resource<string> typeResource = plugin.PluginType.GetTypeStringResource();
                string type = typeResource.Culture.TextInfo.ToTitleCase(typeResource.Value);
                Color typeColor = plugin.PluginType.GetTypeColor();

                ListViewItem item = new ListViewItem
                {
                    Text = plugin.Name,
                    ToolTipText = plugin.Location,
                };
                item.SubItems.Add(plugin.Title);
                item.SubItems.Add(CreateColoredSubItem(type, typeColor));
                item.SubItems.Add(plugin.Version);
                item.SubItems.Add(plugin.Description);

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
