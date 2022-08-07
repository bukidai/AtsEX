using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.BveTypes;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Helpers;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx
{
    public abstract partial class AtsExExtensionSet : IDisposable
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<AtsExExtensionSet>("Core");

        private readonly ILoadErrorResolver LoadErrorResolver;

        internal BveHacker BveHacker { get; }

        private protected AtsExExtensionSet(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly, ILoadErrorResolver loadErrorResolver)
        {
            LoadErrorResolver = loadErrorResolver;

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            string pluginHostAssemblyPath = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "AtsEx.PluginHost.dll");
            Assembly pluginHostAssembly = Assembly.LoadFrom(pluginHostAssemblyPath);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            {
                AssemblyName assemblyName = new AssemblyName(e.Name);
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyName.Name + ".dll");
                return File.Exists(path) ? Assembly.LoadFrom(path) : null;
            };

            App.CreateInstance(targetProcess, targetAssembly, executingAssembly, pluginHostAssembly);
            BveHacker = new BveHacker(ProfileForDifferentBveVersionLoaded, loadErrorResolver);

            //DXDynamicTextureHost = new DXDynamicTextureHost();
        }

        private protected abstract void ProfileForDifferentBveVersionLoaded(Version profileVersion);

        public void Dispose()
        {
            BveHacker.Dispose();
        }
    }
}