using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.MapPlugins.StationController
{
    public partial class ControllerForm : Form
    {
        protected Button RemoveLastStationButton;

        protected void InitializeComponent()
        {
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(400, 64);
            Font = new Font("Yu Gothic UI", 9);
            Text = "AtsEX マッププラグイン 停車場リスト編集サンプル";

            RemoveLastStationButton = new Button()
            {
                Left = 16,
                Top = 16,
                Width = 80,
                Text = "駅を減らす",
            };
            RemoveLastStationButton.Click += OnButtonClicked;
            Controls.Add(RemoveLastStationButton);
        }
    }
}
