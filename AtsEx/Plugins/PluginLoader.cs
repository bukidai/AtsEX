using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using Automatic9045.AtsEx.Plugins.Scripting;
using Automatic9045.AtsEx.Plugins.Scripting.CSharp;
using Automatic9045.AtsEx.Plugins.Scripting.IronPython2;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.Plugins
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

        protected readonly BveHacker BveHacker;

        public PluginLoader(BveHacker bveHacker)
        {
            BveHacker = bveHacker;
        }

        public SortedList<string, PluginBase> LoadFromPluginUsing(PluginUsing pluginUsing)
        {
            SortedList<string, PluginBase> plugins = new SortedList<string, PluginBase>();

            foreach (KeyValuePair<Identifier, Assembly> item in pluginUsing.Assemblies)
            {
                List<PluginBase> loadedPlugins = LoadFromAssembly(item.Key, item.Value, pluginUsing.PluginType);
                loadedPlugins.ForEach(plugin => plugins[plugin.Identifier] = plugin);
            }

            foreach (KeyValuePair<Identifier, ScriptPluginPackage> item in pluginUsing.CSharpScriptPackages)
            {
                PluginBuilder pluginBuilder = new PluginBuilder(App.Instance, BveHacker, item.Key.Text);
                plugins[item.Key.Text] = CSharpScriptPlugin.FromPackage(pluginBuilder, pluginUsing.PluginType, item.Value);
            }

            foreach (KeyValuePair<Identifier, ScriptPluginPackage> item in pluginUsing.IronPython2Packages)
            {
                PluginBuilder pluginBuilder = new PluginBuilder(App.Instance, BveHacker, item.Key.Text);
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

                        PluginBase pluginInstance = constructorInfo.Invoke(new object[] { new PluginBuilder(App.Instance, BveHacker, GenerateIdentifier()) }) as PluginBase;
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
    }
}
