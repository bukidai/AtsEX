using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Mackoy.Bvets;

using AtsEx.Launcher;

namespace AtsEx.Caller.InputDevice
{
    public class InputDevice : IInputDevice
    {
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        private const string LauncherName = "AtsEx.Launcher";

        private VersionSelector.AsInputDevice VersionSelector;

        public event InputEventHandler LeverMoved;
        public event InputEventHandler KeyDown;
        public event InputEventHandler KeyUp;

        public InputDevice()
        {
#if DEBUG
            if (!Debugger.IsAttached) Debugger.Launch();
#endif
            string launcherLocation = Path.Combine(Path.GetDirectoryName(Assembly.Location), "AtsEx", LauncherName + ".dll");
            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            {
                AssemblyName assemblyName = new AssemblyName(e.Name);
                return assemblyName.Name == LauncherName ? Assembly.LoadFrom(launcherLocation) : null;
            };

            Load();

            void Load()
            {
                VersionSelector = new VersionSelector.AsInputDevice(Assembly);

                VersionSelector.CoreHost.LeverMoved += (sender, e) => LeverMoved?.Invoke(this, new Mackoy.Bvets.InputEventArgs(e.Axis, e.Value));
                VersionSelector.CoreHost.KeyDown += (sender, e) => KeyDown?.Invoke(this, new Mackoy.Bvets.InputEventArgs(e.Axis, e.Value));
                VersionSelector.CoreHost.KeyUp += (sender, e) => KeyUp?.Invoke(this, new Mackoy.Bvets.InputEventArgs(e.Axis, e.Value));
            }
        }

        public void Dispose() => VersionSelector?.CoreHost.Dispose();
        public void Configure(IWin32Window owner) => VersionSelector.CoreHost.Configure(owner);
        public void Load(string settingsPath) => VersionSelector.CoreHost.Load(settingsPath);
        public void SetAxisRanges(int[][] ranges) => VersionSelector.CoreHost.SetAxisRanges(ranges);
        public void Tick() => VersionSelector.CoreHost.Tick();
    }
}
