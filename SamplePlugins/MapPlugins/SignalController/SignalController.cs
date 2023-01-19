using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Plugins;

using AtsEx.Extensions.SignalPatch;

namespace AtsEx.Samples.MapPlugins.SignalController
{
    [PluginType(PluginType.MapPlugin)]
    public class SignalController : AssemblyPluginBase
    {
        private SignalPatch SignalPatch;
        private int SignalIndex = 0;

        public SignalController(PluginBuilder builder) : base(builder)
        {
            BveHacker.ScenarioCreated += e =>
            {
                SectionManager sectionManager = e.Scenario.SectionManager;
                Section section = sectionManager.Sections[2] as Section;

                SignalPatch = Extensions.GetExtension<ISignalPatchFactory>().Patch(nameof(SignalPatch), sectionManager, section, source => SignalIndex);
            };

            Native.NativeKeys.AtsKeys[NativeAtsKeyName.D].Pressed += OnDPressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.E].Pressed += OnEPressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.F].Pressed += OnFPressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.G].Pressed += OnGPressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.H].Pressed += OnHPressed;
        }

        private void OnDPressed(object sender, EventArgs e) => SignalIndex = 0;
        private void OnEPressed(object sender, EventArgs e) => SignalIndex = 1;
        private void OnFPressed(object sender, EventArgs e) => SignalIndex = 2;
        private void OnGPressed(object sender, EventArgs e) => SignalIndex = 3;
        private void OnHPressed(object sender, EventArgs e) => SignalIndex = 4;

        public override void Dispose()
        {
            SignalPatch?.Dispose();

            Native.NativeKeys.AtsKeys[NativeAtsKeyName.D].Pressed -= OnDPressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.E].Pressed -= OnEPressed;
        }

        public override TickResult Tick(TimeSpan elapsed) => new MapPluginTickResult();
    }
}
