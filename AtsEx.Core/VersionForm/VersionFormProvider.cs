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

namespace Automatic9045.AtsEx
{
    internal sealed class VersionFormProvider : IDisposable
    {
        private readonly BveHacker BveHacker;

        private ContextMenuHacker ContextMenuHacker = new ContextMenuHacker();

        private VersionForm Form = null;
        private ToolStripMenuItem MenuItem;


        public VersionFormProvider(BveHacker bveHacker)
        {
            BveHacker = bveHacker;

            MenuItem = ContextMenuHacker.AddClickableMenuItem($"{App.Instance.ProductShortName} バージョン情報...", MenuItemClick);

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

        public void Intialize(IEnumerable<AtsExPluginInfo> plugins)
        {
            Form.SetPluginDetails(plugins);
        }

        public void Dispose()
        {
            Form.FormClosing -= FormClosing;
            Form.Close();
            Form.Dispose();

            ContextMenuHacker.Dispose();
        }
    }
}
