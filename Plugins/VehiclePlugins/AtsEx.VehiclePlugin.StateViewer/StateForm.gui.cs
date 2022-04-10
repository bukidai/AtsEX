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

namespace Automatic9045.VehiclePlugins.StateViewer
{
    public partial class StateForm : Form
    {
        protected Label ScenarioPathKey;
        protected Label ScenarioPathValue;

        protected Label VehiclePathKey;
        protected Label VehiclePathValue;

        protected Label VehicleTitleKey;
        protected Label VehicleTitleValue;

        protected Label RoutePathKey;
        protected Label RoutePathValue;

        protected Label RouteTitleKey;
        protected Label RouteTitleValue;

        protected Label TimeKey;
        protected TextBox TimeValue;

        protected Label LocationKey;
        protected TextBox LocationValue;

        protected Label SpeedKey;
        protected TextBox SpeedValue;

        protected Label DisplaySpeedKey;
        protected Label DisplaySpeedValue;

        protected Button RemoveLastStationButton;

        protected void InitializeComponent()
        {
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(800, 600);
            Font = new Font("Yu Gothic UI", 9);
            Text = "AtsEX 車両プラグイン 状態取得・設定サンプル";


            ScenarioPathKey = new Label()
            {
                Left = 16,
                Top = 16,
                Width = 80,
                Text = "ｼﾅﾘｵﾌｧｲﾙﾊﾟｽ",
            };
            Controls.Add(ScenarioPathKey);

            ScenarioPathValue = new Label()
            {
                Left = 112,
                Top = 16,
                Width = 160,
                Text = AtsExPlugin.BveHacker.CurrentScenarioInfo.Path,
            };
            Controls.Add(ScenarioPathValue);


            VehiclePathKey = new Label()
            {
                Left = 16,
                Top = 48,
                Width = 80,
                Text = "車両ﾌｧｲﾙﾊﾟｽ",
            };
            Controls.Add(VehiclePathKey);

            VehiclePathValue = new Label()
            {
                Left = 112,
                Top = 48,
                Width = 560,
                Text = AtsExPlugin.BveHacker.CurrentScenarioInfo.VehicleFiles.SelectedFile.Path,
            };
            Controls.Add(VehiclePathValue);


            VehicleTitleKey = new Label()
            {
                Left = 16,
                Top = 80,
                Width = 80,
                Text = "車両名",
            };
            Controls.Add(VehicleTitleKey);

            VehicleTitleValue = new Label()
            {
                Left = 112,
                Top = 80,
                Width = 560,
                Text = AtsExPlugin.BveHacker.CurrentScenarioInfo.VehicleTitle,
            };
            Controls.Add(VehicleTitleValue);


            RoutePathKey = new Label()
            {
                Left = 16,
                Top = 112,
                Width = 80,
                Text = "ﾏｯﾌﾟﾌｧｲﾙﾊﾟｽ",
            };
            Controls.Add(RoutePathKey);

            RoutePathValue = new Label()
            {
                Left = 112,
                Top = 112,
                Width = 560,
                Text = AtsExPlugin.BveHacker.CurrentScenarioInfo.RouteFiles.SelectedFile.Path,
            };
            Controls.Add(RoutePathValue);


            RouteTitleKey = new Label()
            {
                Left = 16,
                Top = 144,
                Width = 80,
                Text = "路線名",
            };
            Controls.Add(RouteTitleKey);

            RouteTitleValue = new Label()
            {
                Left = 112,
                Top = 144,
                Width = 560,
                Text = AtsExPlugin.BveHacker.CurrentScenarioInfo.RouteTitle,
            };
            Controls.Add(RouteTitleValue);


            TimeKey = new Label()
            {
                Left = 16,
                Top = 176,
                Width = 80,
                Text = "時刻",
            };
            Controls.Add(TimeKey);

            TimeValue = new TextBox()
            {
                Name = nameof(TimeValue),
                Left = 112,
                Top = 176 - 2,
                Width = 160,
            };
            TimeValue.KeyDown += OnKeyDown;
            Controls.Add(TimeValue);


            LocationKey = new Label()
            {
                Left = 16,
                Top = 208,
                Width = 80,
                Text = "位置",
            };
            Controls.Add(LocationKey);

            LocationValue = new TextBox()
            {
                Name = nameof(LocationValue),
                Left = 112,
                Top = 208 - 2,
                Width = 160,
            };
            LocationValue.KeyDown += OnKeyDown;
            Controls.Add(LocationValue);


            SpeedKey = new Label()
            {
                Left = 16,
                Top = 240,
                Width = 80,
                Text = "速度",
            };
            Controls.Add(SpeedKey);

            SpeedValue = new TextBox()
            {
                Name = nameof(SpeedValue),
                Left = 112,
                Top = 240 - 2,
                Width = 160,
            };
            SpeedValue.KeyDown += OnKeyDown;
            Controls.Add(SpeedValue);


            DisplaySpeedKey = new Label()
            {
                Left = 16,
                Top = 272,
                Width = 80,
                Text = "表示速度",
            };
            Controls.Add(DisplaySpeedKey);

            DisplaySpeedValue = new Label()
            {
                Left = 112,
                Top = 272,
                Width = 160,
            };
            DisplaySpeedValue.KeyDown += OnKeyDown;
            Controls.Add(DisplaySpeedValue);

            RemoveLastStationButton = new Button()
            {
                Left = 16,
                Top = 304,
                Width = 80,
                Text = "駅を減らす",
            };
            RemoveLastStationButton.Click += OnButtonClicked;
            Controls.Add(RemoveLastStationButton);
        }
    }
}
