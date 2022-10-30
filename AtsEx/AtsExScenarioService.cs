using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.Handles;
using AtsEx.Input;
using AtsEx.Plugins;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.Handles;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Plugins;

namespace AtsEx
{
    internal abstract partial class AtsExScenarioService : IDisposable
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<AtsExScenarioService>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> VehiclePluginTickResultTypeInvalid { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> MapPluginTickResultTypeInvalid { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static AtsExScenarioService()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private readonly AtsEx AtsEx;

        private readonly NativeImpl Native;
        private readonly PluginSet Plugins;

        private readonly BveHacker BveHacker;

        protected AtsExScenarioService(AtsEx atsEx, PluginSourceSet vehiclePluginUsing, PluginHost.Native.VehicleSpec vehicleSpec)
        {
            AtsEx = atsEx;
            BveHacker = AtsEx.BveHacker;

            Native = new NativeImpl(vehicleSpec);

            PluginLoadErrorResolver loadErrorResolver = new PluginLoadErrorResolver(BveHacker.LoadErrorManager);

            Plugins.PluginLoader pluginLoader = new Plugins.PluginLoader(Native, BveHacker, AtsEx.Extensions);
            Dictionary<string, PluginBase> vehiclePlugins = null;
            Dictionary<string, PluginBase> mapPlugins = null;
            try
            {
                {
                    vehiclePlugins = pluginLoader.Load(vehiclePluginUsing);
                }

                {
                    Map map = Map.Load(BveHacker.ScenarioInfo.RouteFiles.SelectedFile.Path, pluginLoader, loadErrorResolver);
                    mapPlugins = map.LoadedPlugins;

                    IEnumerable<LoadError> removeTargetErrors = BveHacker.LoadErrorManager.Errors.Where(error =>
                    {
                        if (error.Text.Contains("[[NOMPI]]")) return true;

                        bool isMapPluginUsingError = map.MapPluginUsingErrors.Contains(error, new LoadErrorEqualityComparer());
                        return isMapPluginUsingError;
                    });
                    foreach (LoadError error in removeTargetErrors)
                    {
                        BveHacker.LoadErrorManager.Errors.Remove(error);
                    }
                }
            }
            catch (Exception ex)
            {
                loadErrorResolver.Resolve(ex);
            }
            finally
            {
                if (vehiclePlugins is null) vehiclePlugins = new Dictionary<string, PluginBase>();
                if (mapPlugins is null) mapPlugins = new Dictionary<string, PluginBase>();

                Plugins = new PluginSet(vehiclePlugins, mapPlugins);
                pluginLoader.SetPluginSetToLoadedPlugins(Plugins);

                AtsEx.VersionFormProvider.SetScenario(Plugins.Select(item => item.Value));
            }

            BveHacker.SetScenario(Native);
        }

        public void Dispose()
        {
            AtsEx.VersionFormProvider.UnsetScenario();

            foreach (KeyValuePair<string, PluginBase> plugin in Plugins)
            {
                plugin.Value.Dispose();
            }
        }

        public void Started(BrakePosition defaultBrakePosition)
        {
            Native.InvokeStarted(defaultBrakePosition);
        }

        public HandlePositionSet Tick(TimeSpan elapsed, PluginHost.Native.VehicleState vehicleState)
        {
            Native.VehicleState = vehicleState;

            BveHacker.Tick(elapsed);

            int powerNotch = Native.Handles.Power.Notch;
            int brakeNotch = Native.Handles.Brake.Notch;
            ReverserPosition reverserPosition = Native.Handles.Reverser.Position;

            int? atsPowerNotch = null;
            int? atsBrakeNotch = null;
            ReverserPosition? atsReverserPosition = null;
            ConstantSpeedCommand? atsConstantSpeedCommand = null;

            foreach (PluginBase plugin in Plugins[PluginType.VehiclePlugin].Values)
            {
                TickResult tickResult = plugin.Tick(elapsed);
                if (!(tickResult is VehiclePluginTickResult vehiclePluginTickResult))
                {
                    throw new InvalidOperationException(string.Format(Resources.Value.VehiclePluginTickResultTypeInvalid.Value,
                       $"{nameof(PluginBase)}.{nameof(PluginBase.Tick)}", nameof(VehiclePluginTickResult)));
                }

                HandleCommandSet commandSet = vehiclePluginTickResult.HandleCommandSet;

                if (atsPowerNotch is null) atsPowerNotch = commandSet.PowerCommand.GetOverridenNotch(powerNotch);
                if (atsBrakeNotch is null) atsBrakeNotch = commandSet.BrakeCommand.GetOverridenNotch(brakeNotch);
                if (atsReverserPosition is null) atsReverserPosition = commandSet.ReverserCommand.GetOverridenPosition(reverserPosition);
                if (atsConstantSpeedCommand is null) atsConstantSpeedCommand = commandSet.ConstantSpeedCommand;
            }

            foreach (PluginBase plugin in Plugins[PluginType.MapPlugin].Values)
            {
                TickResult tickResult = plugin.Tick(elapsed);
                if (!(tickResult is MapPluginTickResult))
                {
                    throw new InvalidOperationException(string.Format(Resources.Value.MapPluginTickResultTypeInvalid.Value,
                       $"{nameof(PluginBase)}.{nameof(PluginBase.Tick)}", nameof(MapPluginTickResult)));
                }
            }

            return new HandlePositionSet(atsPowerNotch ?? powerNotch, atsBrakeNotch ?? brakeNotch, atsReverserPosition ?? reverserPosition, atsConstantSpeedCommand ?? ConstantSpeedCommand.Continue);
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


        private class LoadErrorEqualityComparer : IEqualityComparer<LoadError>
        {
            public bool Equals(LoadError x, LoadError y) => x.SenderFileName == y.SenderFileName && x.LineIndex == y.LineIndex && x.CharIndex == y.CharIndex;
            public int GetHashCode(LoadError obj) => obj.GetHashCode();
        }
    }
}
