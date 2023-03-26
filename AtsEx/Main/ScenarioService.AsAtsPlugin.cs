using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.Plugins;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Native;
using AtsEx.PluginHost.Plugins;

namespace AtsEx
{
    internal partial class ScenarioService
    {
        internal sealed class AsAtsPlugin : ScenarioService
        {
            public AsAtsPlugin(AtsEx.AsAtsPlugin atsEx, Assembly callerAssembly, VehicleSpec vehicleSpec, string versionWarningText)
                : base(atsEx, LoadVehiclePluginUsing(callerAssembly), vehicleSpec)
            {
                if (BveHacker.BveTypes.ProfileVersion != App.Instance.BveVersion && !_PluginService.UseAtsExExtensions)
                {
                    LoadError removeTargetError = BveHacker.LoadErrorManager.Errors.FirstOrDefault(error => error.Text == versionWarningText);
                    if (!(removeTargetError is null))
                    {
                        BveHacker.LoadErrorManager.Errors.Remove(removeTargetError);
                    }
                }
            }

            private static PluginSourceSet LoadVehiclePluginUsing(Assembly callerAssembly)
            {
                string vehiclePluginUsingPath = Path.Combine(Path.GetDirectoryName(callerAssembly.Location), Path.GetFileNameWithoutExtension(callerAssembly.Location) + ".VehiclePluginUsing.xml");
                PluginSourceSet vehiclePluginUsing = PluginSourceSet.FromPluginUsing(PluginType.VehiclePlugin, vehiclePluginUsingPath);

                return vehiclePluginUsing;
            }
        }
    }
}
