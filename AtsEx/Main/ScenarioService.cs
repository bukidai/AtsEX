using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;
using UnembeddedResources;

using AtsEx.Handles;
using AtsEx.Input;
using AtsEx.Plugins;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Native;

namespace AtsEx
{
    internal abstract partial class ScenarioService : IDisposable
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<ScenarioService>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> VehiclePluginTickResultTypeInvalid { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> MapPluginTickResultTypeInvalid { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static ScenarioService()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private readonly AtsEx AtsEx;
        private readonly NativeImpl Native;
        private readonly BveHacker BveHacker;

        private readonly PluginService _PluginService;

        protected ScenarioService(AtsEx atsEx, PluginSourceSet vehiclePluginUsing, PluginHost.Native.VehicleSpec vehicleSpec)
        {
            AtsEx = atsEx;
            BveHacker = AtsEx.BveHacker;

            Native = new NativeImpl(vehicleSpec);

            PluginLoader pluginLoader = new PluginLoader(Native, BveHacker, AtsEx.Extensions);
            PluginSet plugins = pluginLoader.Load(vehiclePluginUsing);
            _PluginService = new PluginService(plugins, Native.Handles);

            AtsEx.VersionFormProvider.SetScenario(plugins.Select(item => item.Value));
        }

        public void Dispose()
        {
            AtsEx.VersionFormProvider.UnsetScenario();
            _PluginService.Dispose();
        }

        public void Started(BrakePosition defaultBrakePosition)
        {
            Native.InvokeStarted(defaultBrakePosition);
        }

        public HandlePositionSet Tick(TimeSpan elapsed, VehicleState vehicleState, Native.AtsIoArray panel, Native.AtsIoArray sound)
        {
            Native.VehicleState = vehicleState;

            BveHacker.Tick(elapsed);
            HandlePositionSet tickResult = _PluginService.Tick(elapsed);

            return tickResult;
        }

        public void SetPower(int notch)
        {
            (Native.Handles.Power as PowerHandle).Notch = notch;
        }

        public void SetBrake(int notch)
        {
            (Native.Handles.Brake as BrakeHandle).Notch = notch;
        }

        public void SetReverser(ReverserPosition position)
        {
            (Native.Handles.Reverser as Reverser).Position = position;
        }

        public void KeyDown(NativeAtsKeyName key)
        {
            (Native.NativeKeys.AtsKeys[key] as NativeAtsKey).NotifyPressed();
        }

        public void KeyUp(NativeAtsKeyName key)
        {
            (Native.NativeKeys.AtsKeys[key] as NativeAtsKey).NotifyReleased();
        }

        public void DoorOpened(DoorEventArgs args)
        {
            Native.InvokeDoorOpened(args);
        }

        public void DoorClosed(DoorEventArgs args)
        {
            Native.InvokeDoorClosed(args);
        }

        public void BeaconPassed(BeaconPassedEventArgs args)
        {
            Native.InvokeBeaconPassed(args);
        }
    }
}
