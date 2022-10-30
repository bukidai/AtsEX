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

using AtsEx.Plugins;
using AtsEx.Plugins.Extensions;
using AtsEx.PluginHost;
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
        public IExtensionFactorySet Extensions { get; }

        public VersionFormProvider VersionFormProvider { get; }

        protected AtsEx(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            App.CreateInstance(targetProcess, targetAssembly, executingAssembly);
            BveHacker = new BveHacker(ProfileForDifferentBveVersionLoaded);

            PluginLoadErrorResolver loadErrorResolver = new PluginLoadErrorResolver(BveHacker.LoadErrorManager);

            PluginLoader pluginLoader = new PluginLoader(null, BveHacker, null);
            Dictionary<string, PluginBase> extensionFactories = null;
            try
            {
                string extensionsDirectory = Path.Combine(Path.GetDirectoryName(App.Instance.AtsExAssembly.Location), "Extensions");
                Directory.CreateDirectory(extensionsDirectory);

                string pluginUsingPath = Path.Combine(extensionsDirectory, "PluginUsing.xml");
                PluginSourceSet fromPluginUsing = File.Exists(pluginUsingPath)
                    ? PluginSourceSet.FromPluginUsing(PluginType.Extension, pluginUsingPath) : PluginSourceSet.Empty(PluginType.Extension);
                PluginSourceSet fromDirectory = PluginSourceSet.FromDirectory(null, PluginType.Extension, extensionsDirectory);

                extensionFactories = pluginLoader.Load(fromPluginUsing.Concat(null, fromDirectory));
            }
            catch (Exception ex)
            {
                loadErrorResolver.Resolve(ex);
            }
            finally
            {
                if (extensionFactories is null) extensionFactories = new Dictionary<string, PluginBase>();

                Extensions = new ExtensionFactorySet(extensionFactories.Values);
                pluginLoader.SetExtensionFactorySetToLoadedPlugins(Extensions);
            }

            VersionFormProvider = new VersionFormProvider(BveHacker.ContextMenuHacker as ContextMenuHacker, BveHacker.MainFormSource, extensionFactories.Values);
            TextureManager.Initialize();
        }

        protected abstract void ProfileForDifferentBveVersionLoaded(Version profileVersion);

        public virtual void Dispose()
        {
            TextureManager.Clear();

            VersionFormProvider.Dispose();
            BveHacker.Dispose();
        }

        public void Tick(TimeSpan elapsed)
        {
            foreach (PluginBase extension in Extensions)
            {
                TickResult tickResult = extension.Tick(elapsed);
                if (!(tickResult is ExtensionTickResult))
                {
                    throw new InvalidOperationException(string.Format(Resources.Value.ExtensionTickResultTypeInvalid.Value,
                       $"{nameof(PluginBase)}.{nameof(PluginBase.Tick)}", nameof(ExtensionTickResult)));
                }
            }
        }
    }
}