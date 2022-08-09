using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.Plugins;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx
{
    public partial class AtsExScenarioService
    {
        public sealed class AsAtsPlugin : AtsExScenarioService
        {
            public AsAtsPlugin(AtsEx.AsAtsPlugin atsEx, Assembly callerAssembly, VehicleSpec vehicleSpec)
                : base(atsEx, LoadVehiclePluginUsing(callerAssembly), vehicleSpec)
            {
                if (BveHacker.BveTypes.ProfileVersion != App.Instance.BveVersion && VehiclePlugins.Values.All(plugin => !plugin.UseAtsExExtensions))
                {
                    LoadError removeTargetError = BveHacker.LoadErrorManager.Errors.FirstOrDefault(error => error.Text == atsEx.VersionWarningText);
                    if (!(removeTargetError is null))
                    {
                        BveHacker.LoadErrorManager.Errors.Remove(removeTargetError);
                    }
                }
            }

            private static PluginUsing LoadVehiclePluginUsing(Assembly callerAssembly)
            {
                string vehiclePluginUsingPath = Path.Combine(Path.GetDirectoryName(callerAssembly.Location), Path.GetFileNameWithoutExtension(callerAssembly.Location) + ".VehiclePluginUsing.xml");
                PluginUsing vehiclePluginUsing = PluginUsing.Load(PluginType.VehiclePlugin, vehiclePluginUsingPath);

                return vehiclePluginUsing;
            }
        }
    }
}
