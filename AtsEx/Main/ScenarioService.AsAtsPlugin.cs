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

namespace AtsEx
{
    internal partial class ScenarioService
    {
        internal sealed class AsAtsPlugin : ScenarioService
        {
            public AsAtsPlugin(AtsEx.AsAtsPlugin atsEx, PluginSourceSet vehiclePluginUsing, VehicleConfig vehicleConfig, VehicleSpec vehicleSpec, string versionWarningText)
                : base(atsEx, vehiclePluginUsing, vehicleConfig, vehicleSpec)
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
        }
    }
}
