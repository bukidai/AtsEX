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
        private VersionForm Form = null;
        private ToolStripMenuItem MenuItem;


        public VersionFormProvider()
        {
            MenuItem = ContextMenuHacker.Instance.AddClickableMenuItem($"{App.Instance.ProductShortName} バージョン情報...", MenuItemClick);
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            if (!Form.Visible)
            {
                Form.Show(BveHacker.Instance.MainFormSource);
            }

            Form.Focus();
        }

        private void FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Form.Hide();
        }

        private void DisposeForm()
        {
            Form.FormClosing -= FormClosing;
            Form.Close();
            Form.Dispose();
        }

        public void Intialize(IEnumerable<AtsExPluginInfo> plugins)
        {
            if (!(Form is null)) DisposeForm();

            Form = new VersionForm();
            Form.FormClosing += FormClosing;
            Form.SetPluginDetails(plugins);
        }

        public void Dispose() => DisposeForm();
    }
}
