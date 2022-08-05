using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Helpers;
using Automatic9045.AtsEx.PluginHost.Plugins;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx
{
    internal sealed class VersionFormProvider : IDisposable
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<VersionFormProvider>("Core");

        private readonly BveHacker BveHacker;

        private readonly VersionForm Form = null;
        private readonly ToolStripMenuItem MenuItem;


        public VersionFormProvider(BveHacker bveHacker)
        {
            BveHacker = bveHacker;

            ContextMenuHacker contextMenuHacker = BveHacker.ContextMenuHacker as ContextMenuHacker;
            MenuItem = contextMenuHacker.AddClickableMenuItem(string.Format(Resources.GetString("VersionInfoMenuItem").Value, App.Instance.ProductShortName), MenuItemClick, true);

            Form = new VersionForm();
            Form.FormClosing += FormClosing;
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            if (!Form.Visible)
            {
                Form.Show(BveHacker.MainFormSource);
            }

            Form.Focus();
        }

        private void FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Form.Hide();
        }

        public void Intialize(IEnumerable<PluginBase> plugins)
        {
            Form.SetPluginDetails(plugins);
        }

        public void Dispose()
        {
            Form.FormClosing -= FormClosing;
            Form.Close();
            Form.Dispose();
        }
    }
}
