using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Handles;
using Automatic9045.AtsEx.PluginHost.Input.Native;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.MapPlugins.TrainController
{
    public class TrainController : AssemblyPluginBase, IDisposable
    {
        private Train Train;
        private double Speed = 25 / 3.6; // 25[km/h] = (25 / 3.6)[m/s]

        public TrainController(PluginBuilder builder) : base(builder, PluginType.MapPlugin)
        {
            BveHacker.ScenarioCreated += e => Train = e.Scenario.Trains["test"];

            App.NativeKeys.AtsKeys[NativeAtsKeyName.D].Pressed += OnDPressed;
            App.NativeKeys.AtsKeys[NativeAtsKeyName.E].Pressed += OnEPressed;
        }

        private void OnDPressed(object sender, EventArgs e) => Train.TrainInfo.TrackKey = "1";
        private void OnEPressed(object sender, EventArgs e) => Train.TrainInfo.TrackKey = "0";

        public void Dispose()
        {
            App.NativeKeys.AtsKeys[NativeAtsKeyName.D].Pressed -= OnDPressed;
            App.NativeKeys.AtsKeys[NativeAtsKeyName.E].Pressed -= OnEPressed;
        }

        public override HandleCommandSet Tick(TimeSpan elapsed)
        {
            if (App.NativeKeys.AtsKeys[NativeAtsKeyName.F].IsPressed) Speed -= 10.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;
            if (App.NativeKeys.AtsKeys[NativeAtsKeyName.G].IsPressed) Speed += 10.0 * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;

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

            return null;
        }
    }
}
