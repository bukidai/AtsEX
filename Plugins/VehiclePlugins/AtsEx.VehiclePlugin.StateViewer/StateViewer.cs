using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Helpers;

namespace Automatic9045.VehiclePlugins.StateViewer
{
    public class StateViewer : AtsExPluginBase, IDisposable
    {
        private StateForm Form;
        private ToolStripMenuItem MenuItem;

        public StateViewer(HostServiceCollection services) : base(services, PluginType.VehiclePlugin)
        {
            InstanceStore.Initialize(App, BveHacker);

            MenuItem = ContextMenuHacker.Instance.AddCheckableMenuItem("状態ウィンドウを表示", MenuItemCheckedChanged);

            MenuItem.Checked = false;

            Form = new StateForm();
            Form.FormClosing += FormClosing;
            Form.WindowState = FormWindowState.Normal;

            MenuItem.Checked = true;
            BveHacker.MainFormSource.Focus();
        }

        public override void Tick()
        {
            Form?.Tick();
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
        }
    }
}
