using System;
using System.Windows.Forms;

using AtsEx.Extensions.ContextMenuHacker;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Samples.VehiclePlugins.StateViewer {
    // 規定のクラスを継承することでプラグインとみなされます
    [PluginType(PluginType.VehiclePlugin)]
    public class StateViewer : AssemblyPluginBase {
        private readonly StateForm Form;
        private readonly ToolStripMenuItem MenuItem;

        public StateViewer(PluginBuilder services) : base(services) {
            InstanceStore.Initialize(Native, BveHacker);

            // 右クリックメニューに独自のメニューを追加可能
            IContextMenuHacker contextMenuHacker = Extensions.GetExtension&lt;IContextMenuHacker&gt;();
            MenuItem = contextMenuHacker.AddCheckableMenuItem("状態ウィンドウを表示", MenuItemCheckedChanged, ContextMenuItemType.Plugins);

            MenuItem.Checked = false;

            // 独自のフォーム（ウィンドウ）を表示可能
            Form = new StateForm();
            Form.FormClosing += FormClosing;
            Form.WindowState = FormWindowState.Normal;

            MenuItem.Checked = true;
            BveHacker.MainFormSource.Focus();
        }

        public override void Dispose() {
            Form.FormClosing -= FormClosing;
            Form.Close();
            Form.Dispose();
        }

        public override TickResult Tick(TimeSpan elapsed) {
            Form?.Tick();

            return new VehiclePluginTickResult();
        }

        private void MenuItemCheckedChanged(object sender, EventArgs e) {
            if (MenuItem.Checked) {
                Form.Show(BveHacker.MainFormSource);
            } else {
                Form.Hide();
            }
        }

        private void FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            MenuItem.Checked = false;
        }
    }
}   