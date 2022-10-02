using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using UnembeddedResources;

using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx
{
    internal partial class VersionForm : Form
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<VersionForm>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> Caption { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> Description { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> License { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> Website { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> Repository { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PluginListHeader { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PluginListColumnFileName { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PluginListColumnName { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PluginListColumnType { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PluginListColumnVersion { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PluginListColumnDescription { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> OK { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly ResourceSet Resources = new ResourceSet();


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
