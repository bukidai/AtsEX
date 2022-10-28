using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.Native;
using AtsEx.Plugins.Scripting;
using AtsEx.Plugins.Scripting.CSharp;
using AtsEx.Plugins.Scripting.IronPython2;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Plugins
{
    internal class PluginLoader
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<PluginLoader>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PluginClassNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PluginVersionNotSupported { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ConstructorNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> CannotSetIdentifier { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> WrongPluginType { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> MustUseExtensions { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> MaybeBecauseBuiltForDifferentVersion { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();
        private static readonly Version SupportedMinVersion = new Version(0, 12);

        static PluginLoader()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        protected readonly NativeImpl Native;
        protected readonly BveHacker BveHacker;

        public event AllPluginsLoadedEventHandler AllPluginsLoaded;

        public PluginLoader(NativeImpl native, BveHacker bveHacker)
        {
            Native = native;
            BveHacker = bveHacker;
        }

        public Dictionary<string, PluginBase> LoadFromPluginUsing(PluginUsing pluginUsing)
        {
            Dictionary<string, PluginBase> plugins = new Dictionary<string, PluginBase>();

            foreach (KeyValuePair<Identifier, Assembly> item in pluginUsing.Assemblies)
            {
                List<PluginBase> loadedPlugins = LoadFromAssembly(item.Key, item.Value, pluginUsing.PluginType);
                loadedPlugins.ForEach(plugin => plugins[plugin.Identifier] = plugin);
            }

            foreach (KeyValuePair<Identifier, ScriptPluginPackage> item in pluginUsing.CSharpScriptPackages)
            {
                PluginBuilder pluginBuilder = new PluginBuilder(Native, BveHacker, item.Key.Text, this);
                plugins[item.Key.Text] = CSharpScriptPlugin.FromPackage(pluginBuilder, pluginUsing.PluginType, item.Value);
            }

            foreach (KeyValuePair<Identifier, ScriptPluginPackage> item in pluginUsing.IronPython2Packages)
            {
                PluginBuilder pluginBuilder = new PluginBuilder(Native, BveHacker, item.Key.Text, this);
                plugins[item.Key.Text] = IronPython2Plugin.FromPackage(pluginBuilder, pluginUsing.PluginType, item.Value);
            }

            // TODO: ここで他の種類のプラグイン（ネイティブなど）を読み込む

            return plugins;


            List<PluginBase> LoadFromAssembly(Identifier identifier, Assembly assembly, PluginType pluginType)
            {
                string fileName = Path.GetFileName(assembly.Location);

                Version pluginHostVersion = App.Instance.AtsExPluginHostAssembly.GetName().Version;

                AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
                AssemblyName referencedPluginHostAssembly = referencedAssemblies.FirstOrDefault(asm => asm.Name == "AtsEx.PluginHost");
                if (referencedPluginHostAssembly is null)
                {
                    throw new BveFileLoadException(string.Format(Resources.Value.PluginClassNotFound.Value, fileName, nameof(PluginBase), App.Instance.ProductShortName));
                }
                else if (referencedPluginHostAssembly.Version < SupportedMinVersion)
                {
                    throw new BveFileLoadException(string.Format(
                        Resources.Value.PluginVersionNotSupported.Value,
                        fileName, referencedPluginHostAssembly.Version, App.Instance.ProductShortName, pluginHostVersion, SupportedMinVersion.ToString(2)));
                }

                bool isBuiltForDifferentVersion = referencedPluginHostAssembly.Version != pluginHostVersion;

                try
                {
                    Type[] allTypes = assembly.GetTypes();
                    IEnumerable<Type> pluginTypes = allTypes.Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && t.IsSubclassOf(typeof(PluginBase)));
                    if (!pluginTypes.Any())
                    {
                        throw new BveFileLoadException(string.Format(Resources.Value.PluginClassNotFound.Value, fileName, nameof(PluginBase), App.Instance.ProductShortName));
                    }

                    List<(Type, ConstructorInfo)> constructors = new List<(Type, ConstructorInfo)>();
                    foreach (Type type in pluginTypes)
                    {
                        ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(PluginBuilder) });
                        if (constructor is null) continue;

                        constructors.Add((type, constructor));
                    }

                    switch (constructors.Count)
                    {
                        case 0:
                            throw new BveFileLoadException(string.Format(Resources.Value.ConstructorNotFound.Value, fileName, nameof(PluginBase), typeof(PluginBuilder)));

                        case 1:
                            break;

                        default:
                            if (!(identifier is RandomIdentifier))
                            {
                                throw new BveFileLoadException(string.Format(Resources.Value.CannotSetIdentifier.Value, fileName), pluginUsing.Name);
                            }
                            break;
                    }

                    List<PluginBase> constructedPlugins = constructors.ConvertAll(constructor =>
                    {
                        (Type type, ConstructorInfo constructorInfo) = constructor;

                        PluginBase pluginInstance = constructorInfo.Invoke(new object[] { new PluginBuilder(Native, BveHacker, GenerateIdentifier(), this) }) as PluginBase;
                        if (pluginInstance.PluginType != pluginType)
                        {
                            throw new InvalidOperationException(string.Format(Resources.Value.WrongPluginType.Value, pluginType.GetTypeString(), pluginInstance.PluginType.GetTypeString()));
                        }
                        else if (pluginInstance.PluginType == PluginType.MapPlugin && !pluginInstance.UseAtsExExtensions)
                        {
                            throw new NotSupportedException(string.Format(Resources.Value.MustUseExtensions.Value, pluginInstance.PluginType.GetTypeString(), App.Instance.ProductShortName));
                        }

                        return pluginInstance;
                    });

                    return constructedPlugins;


                    string GenerateIdentifier() => constructors.Count == 1 ? identifier.Text : Guid.NewGuid().ToString();
                }
                catch
                {
                    BveHacker.LoadErrorManager.Throw(string.Format(
                        Resources.Value.MaybeBecauseBuiltForDifferentVersion.Value,
                        fileName, pluginHostVersion, App.Instance.ProductShortName));
                    throw;
                }
            }
        }

        public void SetPluginSetToLoadedPlugins(IPluginSet plugins) => AllPluginsLoaded?.Invoke(new AllPluginsLoadedEventArgs(plugins));
    }
}
