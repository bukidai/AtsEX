using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx
{
    internal sealed class App : IApp
    {
        public static App Instance { get; private set; }

        public static void CreateInstance(Assembly bveAssembly, Assembly atsExAssembly, Assembly atsExPluginHostAssembly)
        {
            Instance = new App(bveAssembly, atsExAssembly, atsExPluginHostAssembly);
        }


        private App(Assembly bveAssembly, Assembly atsExAssembly, Assembly atsExPluginHostAssembly)
        {
            AtsExAssembly = Assembly.GetExecutingAssembly();
            BveAssembly = bveAssembly;
            AtsExPluginHostAssembly = atsExPluginHostAssembly;
        }

        public string ProductName { get; } = "AtsEX ATSプラグイン拡張キット for BVE 5 & 6";
        public string ProductShortName { get; } = "AtsEX";

        public Assembly AtsExAssembly { get; }
        public Assembly AtsExPluginHostAssembly { get; }
        public Assembly BveAssembly { get; }

        private List<AtsExPluginInfo> _VehiclePlugins = null;
        public List<AtsExPluginInfo> VehiclePlugins
        {
            get
            {
                if (_VehiclePlugins is null) throw new PropertyNotInitializedException(nameof(VehiclePlugins));
                return _VehiclePlugins;
            }
            set
            {
                _VehiclePlugins = value;
                AllVehiclePluginLoaded?.Invoke(new AllPluginLoadedEventArgs(_VehiclePlugins));
            }
        }

        private List<AtsExPluginInfo> _MapPlugins = null;
        public List<AtsExPluginInfo> MapPlugins
        {
            get
            {
                if (_MapPlugins is null) throw new PropertyNotInitializedException(nameof(MapPlugins));
                return _MapPlugins;
            }
            set
            {
                _MapPlugins = value;
                AllMapPluginLoaded?.Invoke(new AllPluginLoadedEventArgs(_MapPlugins));
            }
        }

        public void InvokeStarted(BrakePosition defaultBrakePosition)
        {
            StartedEventArgs e = new StartedEventArgs(defaultBrakePosition);
            Started?.Invoke(e);
        }

        public event AllPluginLoadedEventHandler AllVehiclePluginLoaded;
        public event AllPluginLoadedEventHandler AllMapPluginLoaded;
        public event StartedEventHandler Started;
    }
}
