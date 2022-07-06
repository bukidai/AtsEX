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

namespace Automatic9045.AtsEx
{
    internal sealed class PluginLoader
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<PluginLoader>("Core");

        private readonly BveHacker BveHacker;

        public PluginLoader(BveHacker bveHacker)
        {
            BveHacker = bveHacker;
        }

        public IEnumerable<PluginInfo> LoadFromList(PluginType pluginType, string listAbsolutePath)
        {
            if (!File.Exists(listAbsolutePath))
            {
                throw new BveFileLoadException(string.Format(Resources.GetString("PluginListNotFound").Value, App.Instance.ProductShortName, listAbsolutePath));
            }

            IEnumerable<PluginListLoader.RecognizedDll> dlls = PluginListLoader.LoadFrom(listAbsolutePath);

            string pluginListName = Path.GetFileName(listAbsolutePath);
            return dlls.Select(dll => Load(pluginType, dll.RelativePath, dll.AbsolutePath, pluginListName, dll.LineIndex)).SelectMany(x => x);
        }

        [UnderConstruction] // AtsExPluginBuilderの作り分け
        public IEnumerable<PluginInfo> Load(PluginType pluginType, string relativePath, string absolutePath, string senderFileName = null, int lineIndex = 0)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.LoadFrom(absolutePath);
            }
            catch (BadImageFormatException)
            {
                int currentBveVersion = App.Instance.BveAssembly.GetName().Version.Major;
                int otherBveVersion = currentBveVersion == 6 ? 5 : 6;
                throw new BveFileLoadException(
                    string.Format(Resources.GetString("BadImageFormat").Value, relativePath, otherBveVersion, App.Instance.ProductShortName, currentBveVersion),
                    senderFileName, lineIndex);
            }

            Type[] allTypes = assembly.GetTypes();
            IEnumerable<Type> pluginTypeCandidates = allTypes.Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && t.IsSubclassOf(typeof(PluginBase)));
            if (!pluginTypeCandidates.Any())
            {
                throw new BveFileLoadException(
                    string.Format(Resources.GetString("PluginClassNotFound").Value, relativePath, nameof(PluginBase), App.Instance.ProductShortName),
                    senderFileName, lineIndex);
            }

            PluginBuilder pluginBuilder = new PluginBuilder(App.Instance)
                .UseAtsExExtensions(BveHacker);

            IEnumerable<PluginInfo> plugins = pluginTypeCandidates.Select(t =>
            {
                ConstructorInfo constructorInfo = t.GetConstructor(new Type[] { typeof(PluginBuilder) });
                if (constructorInfo is null) return null;

                PluginBase pluginInstance = constructorInfo.Invoke(new object[] { pluginBuilder }) as PluginBase;
                if (pluginInstance.PluginType != pluginType) throw new InvalidOperationException(string.Format(Resources.GetString("WrongPluginType").Value, pluginType.GetTypeString(), pluginInstance.PluginType.GetTypeString()));
                if (pluginInstance.PluginType == PluginType.MapPlugin && !pluginInstance.UseAtsExExtensions) throw new NotSupportedException(string.Format(Resources.GetString("MustUseExtensions").Value, pluginInstance.PluginType.GetTypeString(), App.Instance.ProductShortName));

                PluginInfo pluginInfo = new PluginInfo(assembly, t.FullName, pluginInstance);
                return pluginInfo;
            }).Where(plugin => !(plugin is null)).ToArray();
            if (!plugins.Any())
            {
                throw new BveFileLoadException(
                    string.Format(Resources.GetString("ConstructorNotFound").Value, relativePath, nameof(PluginBase), typeof(PluginBuilder)),
                    senderFileName, lineIndex);
            }

            return plugins;
        }
    }
}
