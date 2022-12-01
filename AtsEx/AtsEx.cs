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

            PluginLoader pluginLoader = new PluginLoader(null, BveHacker, null);

            string extensionsDirectory = Path.Combine(Path.GetDirectoryName(App.Instance.AtsExAssembly.Location), "Extensions");
            Directory.CreateDirectory(extensionsDirectory);

            string pluginUsingPath = Path.Combine(extensionsDirectory, "PluginUsing.xml");
            PluginSourceSet fromPluginUsing = File.Exists(pluginUsingPath)
                ? PluginSourceSet.FromPluginUsing(PluginType.Extension, pluginUsingPath) : PluginSourceSet.Empty(PluginType.Extension);
            PluginSourceSet fromDirectory = PluginSourceSet.FromDirectory(null, PluginType.Extension, extensionsDirectory);

            Queue<Exception> exceptionsToResolve = new Queue<Exception>();
            Dictionary<string, PluginBase> extensions = pluginLoader.Load(fromPluginUsing.Concat(null, fromDirectory),
                OnFailedToLoadAssembly, OnFailedToLoadScriptPlugin, OnFailedToLoadScriptPlugin);

            Extensions = new ExtensionSet(extensions.Values);
            ResolveExtensionLoadErrors(exceptionsToResolve);
            pluginLoader.SetExtensionSetToLoadedPlugins(Extensions);

            VersionFormProvider = CreateVersionFormProvider(extensions.Values);


            void OnFailedToLoadAssembly(Assembly assembly, Exception ex)
            {
                Version pluginHostVersion = App.Instance.AtsExPluginHostAssembly.GetName().Version;
                Version referencedPluginHostVersion = assembly.GetReferencedPluginHost().Version;
                if (pluginHostVersion != referencedPluginHostVersion)
                {
                    string assemblyFileName = Path.GetFileName(assembly.Location);

                    string message = string.Format(Resources.Value.MaybeBecauseBuiltForDifferentVersion.Value, pluginHostVersion, App.Instance.ProductShortName);
                    BveFileLoadException additionalInfoException = new BveFileLoadException(message, assemblyFileName);

                    exceptionsToResolve.Enqueue(additionalInfoException);
                }

                exceptionsToResolve.Enqueue(ex);
            }

            void OnFailedToLoadScriptPlugin(ScriptPluginPackage scriptPluginPackage, Exception ex)
            {
                exceptionsToResolve.Enqueue(ex);
            }
        }

        private void ResolveExtensionLoadErrors(Queue<Exception> exceptionsToResolve)
        {
            PluginLoadErrorResolver loadErrorResolver = new PluginLoadErrorResolver(BveHacker.LoadErrorManager);
            try
            {
                while (exceptionsToResolve.Count > 0)
                {
                    Exception exception = exceptionsToResolve.Dequeue();
                    loadErrorResolver.Resolve(exception);
                }
            }
            catch
            {
                throw exceptionsToResolve.Peek();
            }
        }

        private VersionFormProvider CreateVersionFormProvider(IEnumerable<PluginBase> extensions)
            => new VersionFormProvider(BveHacker.MainFormSource, extensions, Extensions.GetExtension<IContextMenuHacker>());

        protected abstract void ProfileForDifferentBveVersionLoaded(Version profileVersion);

        public virtual void Dispose()
        {
            VersionFormProvider.Dispose();

            foreach (PluginBase extension in Extensions)
            {
                extension.Dispose();
            }

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