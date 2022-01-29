using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static string GetTypeString(this PluginType pluginType)
        {
            switch (pluginType)
            {
                case PluginType.VehiclePlugin:
                    return "車両プラグイン";

                case PluginType.MapPlugin:
                    return "マッププラグイン";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
