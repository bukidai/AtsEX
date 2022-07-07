using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.Plugins;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.Plugins
{
    internal class PluginLoader
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(PluginLoader), "Core");

        protected readonly BveHacker BveHacker;
        protected readonly PluginBuilder PluginBuilder;

        public PluginLoader(BveHacker bveHacker)
        {
            BveHacker = bveHacker;

            PluginBuilder = new PluginBuilder(App.Instance)
                .UseAtsExExtensions(BveHacker);
        }

        public IEnumerable<PluginBase> LoadFromPluginUsing(PluginUsing pluginUsing)
        {
            IEnumerable<PluginBase> assemblyPlugins = pluginUsing.Assemblies.
                Select(assembly => LoadFromAssembly(assembly, pluginUsing.PluginType)).
                SelectMany(x => x);

            // TODO: ここで他の種類のプラグイン（スクリプト、ネイティブなど）を読み込む

            IEnumerable<PluginBase> plugins = new[]
            {
                assemblyPlugins,
            }.SelectMany(x => x);

            return plugins;
        }

        protected IEnumerable<PluginBase> LoadFromAssembly(Assembly assembly, PluginType pluginType)
        {
            string fileName = Path.GetFileName(assembly.Location);

            Type[] allTypes = assembly.GetTypes();
            IEnumerable<Type> pluginTypes = allTypes.Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && t.IsSubclassOf(typeof(PluginBase)));
            if (!pluginTypes.Any())
            {
                throw new BveFileLoadException(string.Format(Resources.GetString("PluginClassNotFound").Value, fileName, nameof(PluginBase), App.Instance.ProductShortName));
            }

            IEnumerable<PluginBase> plugins = pluginTypes.Select(t =>
            {
                ConstructorInfo constructorInfo = t.GetConstructor(new Type[] { typeof(PluginBuilder) });
                if (constructorInfo is null) return null;

                PluginBase pluginInstance = constructorInfo.Invoke(new object[] { PluginBuilder }) as PluginBase;
                if (pluginInstance.PluginType != pluginType)
                {
                    throw new InvalidOperationException(string.Format(Resources.GetString("WrongPluginType").Value, pluginType.GetTypeString(), pluginInstance.PluginType.GetTypeString()));
                }
                else if (pluginInstance.PluginType == PluginType.MapPlugin && !pluginInstance.UseAtsExExtensions)
                {
                    throw new NotSupportedException(string.Format(Resources.GetString("MustUseExtensions").Value, pluginInstance.PluginType.GetTypeString(), App.Instance.ProductShortName));
                }

                return pluginInstance;
            }).Where(plugin => !(plugin is null)).ToArray();

            return plugins.Any()
                ? plugins
                : throw new BveFileLoadException(string.Format(Resources.GetString("ConstructorNotFound").Value, fileName, nameof(PluginBase), typeof(PluginBuilder)));
        }
    }
}
