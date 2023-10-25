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
            public AsInputDevice(AtsEx.AsInputDevice atsEx, PluginSourceSet vehiclePluginUsing, VehicleConfig vehicleConfig, VehicleSpec vehicleSpec)
                : base(atsEx, vehiclePluginUsing, vehicleConfig, vehicleSpec)
            {
            }
        }
    }
}
