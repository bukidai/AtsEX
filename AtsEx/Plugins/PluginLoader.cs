using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.Plugins.Scripting;
using Automatic9045.AtsEx.Plugins.Scripting.CSharp;
using Automatic9045.AtsEx.Plugins.Scripting.IronPython2;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Plugins;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.Plugins
{
    internal class PluginLoader
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(PluginLoader), "Core");

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
                List<PluginBase> loadedPlugins = LoadFromAssembly(item.Key, item.Value, pluginUsing.PluginType, pluginUsing.Name);
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
        }

        protected List<PluginBase> LoadFromAssembly(Identifier identifier, Assembly assembly, PluginType pluginType, string pluginUsingName)
        {
            string fileName = Path.GetFileName(assembly.Location);

            Type[] allTypes = assembly.GetTypes();
            IEnumerable<Type> pluginTypes = allTypes.Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && t.IsSubclassOf(typeof(PluginBase)));
            if (!pluginTypes.Any())
            {
                throw new BveFileLoadException(string.Format(Resources.GetString("PluginClassNotFound").Value, fileName, nameof(PluginBase), App.Instance.ProductShortName));
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
                    throw new BveFileLoadException(string.Format(Resources.GetString("ConstructorNotFound").Value, fileName, nameof(PluginBase), typeof(PluginBuilder)));

                case 1:
                    break;

                default:
                    if (!(identifier is RandomIdentifier))
                    {
                        throw new BveFileLoadException(string.Format(Resources.GetString("CannotSetIdentifier").Value, fileName), pluginUsingName);
                    }
                    break;
            }

            List<PluginBase> plugins = new List<PluginBase>();
            foreach ((Type type, ConstructorInfo constructorInfo) in constructors)
            {
                PluginBase pluginInstance = constructorInfo.Invoke(new object[] { new PluginBuilder(App.Instance, BveHacker, GenerateIdentifier()) }) as PluginBase;
                if (pluginInstance.PluginType != pluginType)
                {
                    throw new InvalidOperationException(string.Format(Resources.GetString("WrongPluginType").Value, pluginType.GetTypeString(), pluginInstance.PluginType.GetTypeString()));
                }
                else if (pluginInstance.PluginType == PluginType.MapPlugin && !pluginInstance.UseAtsExExtensions)
                {
                    throw new NotSupportedException(string.Format(Resources.GetString("MustUseExtensions").Value, pluginInstance.PluginType.GetTypeString(), App.Instance.ProductShortName));
                }

                plugins.Add(pluginInstance);
            }

            return plugins;


            string GenerateIdentifier() => constructors.Count == 1 ? identifier.Text : Guid.NewGuid().ToString();
        }
    }
}
