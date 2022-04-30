using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Helpers;

namespace Automatic9045.MapPlugins.StationController
{
    public class StationController : AtsExPluginBase, IDisposable
    {
        private ControllerForm Form;
        private ToolStripMenuItem MenuItem;

        public StationController() : base()
        {
            MenuItem = ContextMenuHacker.Instance.AddCheckableMenuItem("駅編集ウィンドウを表示", MenuItemCheckedChanged);

            App.Started += Started;
        }

        private void Started(StartedEventArgs e)
        {
            if (!(Form is null)) DisposeForm();

            MenuItem.Checked = false;

            Form = new ControllerForm();
            Form.FormClosing += FormClosing;
            Form.WindowState = FormWindowState.Normal;

            MenuItem.Checked = true;
            BveHacker.MainFormSource.Focus();
        }

        public override void Tick()
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

        private void DisposeForm()
        {
            Form.FormClosing -= FormClosing;
            Form.Close();
            Form.Dispose();
        }

        public void Dispose() => DisposeForm();
    }
}
