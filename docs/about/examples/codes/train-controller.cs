using System;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Samples.MapPlugins.TrainController {
    // 規定のクラスを継承することでプラグインとみなされます
    [PluginType(PluginType.MapPlugin)]
    public class TrainController : AssemblyPluginBase {
        private Train Train;
        private double Speed = 25 / 3.6; // 25[km/h] = (25 / 3.6)[m/s]

        public TrainController(PluginBuilder builder) : base(builder) {
            BveHacker.ScenarioCreated += e => Train = e.Scenario.Trains["test"];

            Native.NativeKeys.AtsKeys[NativeAtsKeyName.D].Pressed += OnDPressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.E].Pressed += OnEPressed;
        }

        private void OnDPressed(object sender, EventArgs e) => Train.TrainInfo.TrackKey = "1";
        private void OnEPressed(object sender, EventArgs e) => Train.TrainInfo.TrackKey = "0";

        public override void Dispose() {
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.D].Pressed -= OnDPressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.E].Pressed -= OnEPressed;
        }

        public override TickResult Tick(TimeSpan elapsed) {
            if (Native.NativeKeys.AtsKeys[NativeAtsKeyName.F].IsPressed) Speed -= 10.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;
            if (Native.NativeKeys.AtsKeys[NativeAtsKeyName.G].IsPressed) Speed += 10.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;

            if (Speed > 0) {
                Speed -= 2.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;
                if (Speed < 0) Speed = 0d;
            }
            else if (Speed < 0) {
                Speed += 2.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;
                if (Speed > 0) Speed = 0d;
            }

            Train.Location += Speed * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;
            Train.Speed = Speed;

            return new MapPluginTickResult();
        }
    }
}
