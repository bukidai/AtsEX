using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.PluginHost
{
    /// <summary>
    /// プラグインの種類を指定します。
    /// </summary>
    public enum PluginType
    {
        /// <summary>車両プラグイン。</summary>
        VehiclePlugin,
        /// <summary>マッププラグイン。</summary>
        MapPlugin,
    }

    public static class PluginTypeConverter
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(PluginTypeConverter), "PluginHost");

        public static ResourceInfo<string> GetTypeStringResource(this PluginType pluginType)
        {
            switch (pluginType)
            {
                case PluginType.VehiclePlugin:
                    return Resources.GetString("VehiclePlugin");

                case PluginType.MapPlugin:
                    return Resources.GetString("MapPlugin");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string GetTypeString(this PluginType pluginType) => GetTypeStringResource(pluginType).Value;

        public static Color GetTypeColor(this PluginType pluginType)
        {
            switch (pluginType)
            {
                case PluginType.VehiclePlugin:
                    return Color.LightBlue;

                case PluginType.MapPlugin:
                    return Color.LightGreen;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
