using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Zbx1425.DXDynamicTexture;

using UnembeddedResources;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.BveTypes;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal abstract partial class AtsEx : IDisposable
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<AtsEx>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> AtsExAssemblyLocationIllegal { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> IgnoreAndContinue { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> BveVersionNotSupported { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static AtsEx()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        public BveHacker BveHacker { get; }

        protected AtsEx(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            string pluginHostAssemblyPath = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "AtsEx.PluginHost.dll");
            Assembly pluginHostAssembly = Assembly.LoadFrom(pluginHostAssemblyPath);

            App.CreateInstance(targetProcess, targetAssembly, pluginHostAssembly);
            BveHacker = new BveHacker(ProfileForDifferentBveVersionLoaded);

            TextureManager.Initialize();
        }

        protected abstract void ProfileForDifferentBveVersionLoaded(Version profileVersion);

        public virtual void Dispose()
        {
            TextureManager.Clear();

            BveHacker.Dispose();
        }
    }
}