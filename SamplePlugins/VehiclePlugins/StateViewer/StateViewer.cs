using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AtsEx.PluginHost;
using AtsEx.PluginHost.Handles;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Samples.VehiclePlugins.StateViewer
{
    public class StateViewer : AssemblyPluginBase, IDisposable
    {
        private readonly StateForm Form;
        private readonly ToolStripMenuItem MenuItem;

        public StateViewer(PluginBuilder services) : base(services, PluginType.VehiclePlugin)
        {
            InstanceStore.Initialize(BveHacker);

            MenuItem = BveHacker.ContextMenuHacker.AddCheckableMenuItem("状態ウィンドウを表示", MenuItemCheckedChanged);

            MenuItem.Checked = false;

            Form = new StateForm();
            Form.FormClosing += FormClosing;
            Form.WindowState = FormWindowState.Normal;

            MenuItem.Checked = true;
            BveHacker.MainFormSource.Focus();
        }

        public override void Dispose()
        {
            Form.FormClosing -= FormClosing;
            Form.Close();
            Form.Dispose();
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            Form?.Tick();

            return new VehiclePluginTickResult(HandleCommandSet.DoNothing);
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
    }
}
