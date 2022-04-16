using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.VehiclePlugins.StateViewer
{
    public class StateViewer : AtsExPluginBase, IDisposable
    {
        private StateForm Form;
        private ToolStripMenuItem MenuItem;

        public StateViewer() : base()
        {
            MenuItem = BveHacker.AddCheckableMenuItemToContextMenu("状態ウィンドウを表示", MenuItemCheckedChanged);

            App.Started += Started;
        }

        private void Started(StartedEventArgs e)
        {
            if (!(Form is null)) DisposeForm();

            MenuItem.Checked = false;

            Form = new StateForm();
            Form.FormClosing += FormClosing;
            Form.WindowState = FormWindowState.Normal;

            MenuItem.Checked = true;
            BveHacker.MainForm.Focus();
        }

        public override void Tick()
        {
            Form.Tick();
        }

        private void MenuItemCheckedChanged(object sender, EventArgs e)
        {
            if (MenuItem.Checked)
            {
                Form.Show(BveHacker.MainForm);
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

        private void DisposeForm()
        {
            Form.FormClosing -= FormClosing;
            Form.Close();
            Form.Dispose();
        }

        public void Dispose() => DisposeForm();
    }
}
