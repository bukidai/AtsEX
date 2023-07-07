using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Plugins;
using AtsEx.PluginHost.Native;
using AtsEx.PluginHost.Plugins;

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
        }
    }
}
