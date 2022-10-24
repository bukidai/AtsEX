using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.Handles;
using AtsEx.Input;
using AtsEx.PluginHost;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.Input.Native;
using AtsEx.PluginHost.Native;
using AtsEx.PluginHost.Plugins;

namespace AtsEx
{
    internal sealed class App : IApp
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<App>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ProductName { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ProductShortName { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        public static App Instance { get; private set; }

        static App()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private App(Process targetProcess, Assembly bveAssembly, Assembly atsExPluginHostAssembly)
        {
            Process = targetProcess;
            BveAssembly = bveAssembly;
            AtsExAssembly = Assembly.GetExecutingAssembly();
            AtsExPluginHostAssembly = atsExPluginHostAssembly;
            BveVersion = BveAssembly.GetName().Version;
        }

        public static void CreateInstance(Process targetProcess, Assembly bveAssembly, Assembly atsExPluginHostAssembly)
            => Instance = new App(targetProcess, bveAssembly, atsExPluginHostAssembly);

        public string ProductName { get; } = Resources.Value.ProductName.Value;
        public string ProductShortName { get; } = Resources.Value.ProductShortName.Value;

        public Process Process { get; }
        public Assembly AtsExAssembly { get; }
        public Assembly AtsExPluginHostAssembly { get; }
        public Assembly BveAssembly { get; }
        public Version BveVersion { get; }
    }
}
