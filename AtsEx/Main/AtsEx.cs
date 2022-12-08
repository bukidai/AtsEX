using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.Plugins;
using AtsEx.Plugins.Extensions;
using AtsEx.Plugins.Scripting;

using AtsEx.Extensions.ContextMenuHacker;
using AtsEx.PluginHost;
using AtsEx.PluginHost.LoadErrorManager;
using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

namespace AtsEx
{
    internal abstract partial class AtsEx : IDisposable
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<AtsEx>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> AtsExAssemblyLocationIllegal { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> IgnoreAndContinue { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> BveVersionNotSupported { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ExtensionTickResultTypeInvalid { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> MaybeBecauseBuiltForDifferentVersion { get; private set; }

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

        private readonly ExtensionService _ExtensionService;

        public BveHacker BveHacker { get; }
        public IExtensionSet Extensions { get; }

        public VersionFormProvider VersionFormProvider { get; }

        protected AtsEx(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            {
                AssemblyName assemblyName = new AssemblyName(e.Name);
                if (assemblyName.Name == "SlimDX")
                {
                    Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly assembly in loadedAssemblies)
                    {
                        if (assembly.GetName().Name == "SlimDX") return assembly;
                    }
                }

                return null;
            };

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            App.CreateInstance(targetProcess, targetAssembly, executingAssembly);
            BveHacker = new BveHacker(ProfileForDifferentBveVersionLoaded);

            ExtensionLoader extensionLoader = new ExtensionLoader(BveHacker);
            Extensions = extensionLoader.Load();
            _ExtensionService = new ExtensionService(Extensions);

            VersionFormProvider = CreateVersionFormProvider(Extensions);
        }

        private VersionFormProvider CreateVersionFormProvider(IEnumerable<PluginBase> extensions)
            => new VersionFormProvider(BveHacker.MainFormSource, extensions, Extensions.GetExtension<IContextMenuHacker>());

        protected abstract void ProfileForDifferentBveVersionLoaded(Version profileVersion);

        public virtual void Dispose()
        {
            VersionFormProvider.Dispose();
            _ExtensionService.Dispose();
            BveHacker.Dispose();
        }

        public void Tick(TimeSpan elapsed)
        {
            _ExtensionService.Tick(elapsed);
        }
    }
}