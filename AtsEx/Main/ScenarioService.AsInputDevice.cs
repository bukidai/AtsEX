using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.Plugins;
using AtsEx.PluginHost.Native;
using AtsEx.PluginHost.Plugins;
using AtsEx.Handles;

namespace AtsEx
{
    internal partial class ScenarioService
    {
        internal sealed class AsInputDevice : ScenarioService
        {
            public AsInputDevice(AtsEx.AsInputDevice atsEx, PluginSourceSet loadedVehiclePluginUsing, VehicleConfig vehicleConfig, VehicleSpec vehicleSpec)
                : base(atsEx,
                      loadedVehiclePluginUsing ?? LoadVehiclePluginUsing(atsEx.BveHacker.ScenarioInfo.VehicleFiles.SelectedFile.Path),
                      vehicleConfig ?? LoadVehicleConfig(atsEx.BveHacker.ScenarioInfo.VehicleFiles.SelectedFile.Path),
                      vehicleSpec)
            {
            }

            private static PluginSourceSet LoadVehiclePluginUsing(string vehiclePath)
            {
                string path = Path.Combine(Path.GetDirectoryName(vehiclePath), Path.GetFileNameWithoutExtension(vehiclePath) + ".VehiclePluginUsing.xml");
                if (File.Exists(path))
                {
                    PluginSourceSet vehiclePluginUsing = PluginSourceSet.FromPluginUsing(PluginType.VehiclePlugin, true, path);
                    return vehiclePluginUsing;
                }
                else
                {
                    return PluginSourceSet.Empty(PluginType.VehiclePlugin);
                }
            }

            private static VehicleConfig LoadVehicleConfig(string vehiclePath)
            {
                string path = Path.Combine(Path.GetDirectoryName(vehiclePath), Path.GetFileNameWithoutExtension(vehiclePath) + ".VehicleConfig.xml");
                if (File.Exists(path))
                {
                    VehicleConfig vehicleConfig = VehicleConfig.LoadFrom(path);
                    return vehicleConfig;
                }
                else
                {
                    return VehicleConfig.Default;
                }
            }

            public override HandlePositionSet Tick(TimeSpan elapsed, VehicleState vehicleState, IList<int> panel, IList<int> sound)
            {
                HandlePositionSet handlePositionSet = base.Tick(elapsed, vehicleState, panel, sound);

                HandleSet atsHandles = AtsEx.BveHacker.Scenario.Vehicle.Instruments.PluginLoader.AtsHandles;
                atsHandles.BrakeNotch = handlePositionSet.Brake;
                atsHandles.PowerNotch = handlePositionSet.Power;
                atsHandles.ReverserPosition = handlePositionSet.ReverserPosition;
                atsHandles.ConstantSpeedMode = (ConstantSpeedMode)handlePositionSet.ConstantSpeed;

                return handlePositionSet;
            }
        }
    }
}
