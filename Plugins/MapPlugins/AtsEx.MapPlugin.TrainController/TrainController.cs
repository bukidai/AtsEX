using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.Handles;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Plugins;

namespace Automatic9045.MapPlugins.TrainController
{
    public class TrainController : AssemblyPluginBase, IDisposable
    {
        private Train Train;
        private double Speed = 25 / 3.6; // 25[km/h] = (25 / 3.6)[m/s]

        public TrainController(PluginBuilder builder) : base(builder, PluginType.MapPlugin)
        {
            BveHacker.ScenarioCreated += e =>
            {
                if (!e.Scenario.Trains.ContainsKey("test"))
                {
                    throw new BveFileLoadException("キーが 'test' の他列車が見つかりませんでした。", "TrainController");
                }

                Train = e.Scenario.Trains["test"];
            };

            ScenarioService.NativeKeys.AtsKeys[NativeAtsKeyName.D].Pressed += OnDPressed;
            ScenarioService.NativeKeys.AtsKeys[NativeAtsKeyName.E].Pressed += OnEPressed;
        }

        private void OnDPressed(object sender, EventArgs e) => Train.TrainInfo.TrackKey = "1";
        private void OnEPressed(object sender, EventArgs e) => Train.TrainInfo.TrackKey = "0";

        public void Dispose()
        {
            ScenarioService.NativeKeys.AtsKeys[NativeAtsKeyName.D].Pressed -= OnDPressed;
            ScenarioService.NativeKeys.AtsKeys[NativeAtsKeyName.E].Pressed -= OnEPressed;
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            if (ScenarioService.NativeKeys.AtsKeys[NativeAtsKeyName.F].IsPressed) Speed -= 10.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;
            if (ScenarioService.NativeKeys.AtsKeys[NativeAtsKeyName.G].IsPressed) Speed += 10.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;

            if (Speed > 0)
            {
                Speed -= 2.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;
                if (Speed < 0) Speed = 0d;
            }
            else if (Speed < 0)
            {
                Speed += 2.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;
                if (Speed > 0) Speed = 0d;
            }

            Train.Location += Speed * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;
            Train.Speed = Speed;

            return new MapPluginTickResult();
        }
    }
}
