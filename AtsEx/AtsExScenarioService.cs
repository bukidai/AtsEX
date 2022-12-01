using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;
using UnembeddedResources;

using AtsEx.Handles;
using AtsEx.Input;
using AtsEx.Plugins;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.MapStatements;
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
                    PluginHost.MapStatements.Identifier mapPluginUsingIdentifier = new PluginHost.MapStatements.Identifier(Namespace.Root, "mappluginusing");
                    IEnumerable<IHeader> mapPluginUsingHeaders = BveHacker.MapHeaders.GetAll(mapPluginUsingIdentifier);

                    foreach (IHeader header in mapPluginUsingHeaders)
                    {
                        string mapPluginUsingPath = Path.Combine(Path.GetDirectoryName(BveHacker.ScenarioInfo.RouteFiles.SelectedFile.Path), header.Argument);
                        PluginSourceSet mapPluginUsing = PluginSourceSet.FromPluginUsing(PluginType.MapPlugin, mapPluginUsingPath);

                        Dictionary<string, PluginBase> loadedMapPlugins = pluginLoader.Load(mapPluginUsing);
                        AddRangeToMapPlugins(loadedMapPlugins);
                    }


                    void AddRangeToMapPlugins(Dictionary<string, PluginBase> plugins)
                    {
                        if (mapPlugins is null)
                        {
                            mapPlugins = plugins;
                        }
                        else
                        {
                            foreach (KeyValuePair<string, PluginBase> plugin in plugins)
                            {
                                mapPlugins.Add(plugin.Key, plugin.Value);
                            }
                        }
                    }
                }

                {
                    IEnumerable<LoadError> removeTargetErrors = BveHacker.LoadErrorManager.Errors.Where(error =>
                    {
                        bool isTargetError = BveHacker._MapHeaders.Any(header => header.LineIndex == error.LineIndex && header.CharIndex == header.CharIndex);
                        return isTargetError;
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

            CommandBuilder commandBuilder = new CommandBuilder(Native.Handles);

            foreach (PluginBase plugin in Plugins[PluginType.VehiclePlugin].Values)
            {
                TickResult tickResult = plugin.Tick(elapsed);
                if (!(tickResult is VehiclePluginTickResult vehiclePluginTickResult))
                {
                    throw new InvalidOperationException(string.Format(Resources.Value.VehiclePluginTickResultTypeInvalid.Value,
                       $"{nameof(PluginBase)}.{nameof(PluginBase.Tick)}", nameof(VehiclePluginTickResult)));
                }

                commandBuilder.Override(vehiclePluginTickResult);
            }

            foreach (PluginBase plugin in Plugins[PluginType.MapPlugin].Values)
            {
                TickResult tickResult = plugin.Tick(elapsed);
                if (!(tickResult is MapPluginTickResult mapPluginTickResult))
                {
                    throw new InvalidOperationException(string.Format(Resources.Value.MapPluginTickResultTypeInvalid.Value,
                       $"{nameof(PluginBase)}.{nameof(PluginBase.Tick)}", nameof(MapPluginTickResult)));
                }

                commandBuilder.Override(mapPluginTickResult);
            }

            return commandBuilder.Compile();
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
    }
}
