using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Helpers;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.MapPlugins.StationController
{
    public class StationController : AssemblyPluginBase, IDisposable
    {
        private readonly ContextMenuHacker ContextMenuHacker = new ContextMenuHacker();

        private readonly ControllerForm Form;
        private readonly ToolStripMenuItem MenuItem;

        public StationController(PluginBuilder services) : base(services, PluginType.MapPlugin)
        {
            InstanceStore.Initialize(App, BveHacker);

            MenuItem = ContextMenuHacker.AddCheckableMenuItem("駅編集ウィンドウを表示", MenuItemCheckedChanged);

            MenuItem.Checked = false;

            Form = new ControllerForm();
            Form.FormClosing += FormClosing;
            Form.WindowState = FormWindowState.Normal;

            MenuItem.Checked = true;
            BveHacker.MainFormSource.Focus();
        }

        public override void Tick(TimeSpan elapsed)
        {
        }

        private void MenuItemCheckedChanged(object sender, EventArgs e)
        {
            if (MenuItem.Checked)
            {
                Form.Show(BveHacker.MainFormSource);
            }
            else
            {
                Form.Hide();
            }
        }

        private void FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            MenuItem.Checked = false;
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
