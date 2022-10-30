using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Plugins;
using AtsEx.PluginHost;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.Native;
using AtsEx.PluginHost.Plugins;

namespace AtsEx
{
    internal partial class AtsExScenarioService
    {
        internal sealed class AsAtsPlugin : AtsExScenarioService
        {
            public AsAtsPlugin(AtsEx.AsAtsPlugin atsEx, Assembly callerAssembly, VehicleSpec vehicleSpec)
                : base(atsEx, LoadVehiclePluginUsing(callerAssembly), vehicleSpec)
            {
                if (BveHacker.BveTypes.ProfileVersion != App.Instance.BveVersion && Plugins.All(item => !item.Value.UseAtsExExtensions))
                {
                    LoadError removeTargetError = BveHacker.LoadErrorManager.Errors.FirstOrDefault(error => error.Text == atsEx.VersionWarningText);
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
