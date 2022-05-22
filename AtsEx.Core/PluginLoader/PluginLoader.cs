using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx
{
    internal class PluginLoader
    {
        public HostServiceCollection HostServiceCollection { get; }

        public PluginLoader()
        {
            HostServiceCollection = new HostServiceCollection(App.Instance, BveHacker.Instance);
        }

        public IEnumerable<AtsExPluginInfo> LoadFromList(PluginType pluginType, string listAbsolutePath)
        {
            if (!File.Exists(listAbsolutePath))
            {
                throw new BveFileLoadException($"ATSEx プラグインリスト \"{listAbsolutePath}\" が見つかりません。");
            }

            IEnumerable<PluginListLoader.RecognizedDll> dlls = PluginListLoader.LoadFrom(listAbsolutePath);

            string pluginListName = Path.GetFileName(listAbsolutePath);
            return dlls.Select(dll => Load(pluginType, dll.RelativePath, dll.AbsolutePath, pluginListName, dll.LineIndex)).SelectMany(x => x);
        }

        public IEnumerable<AtsExPluginInfo> Load(PluginType pluginType, string relativePath, string absolutePath, string senderFileName = null, int lineIndex = 0)
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
                    $"\"{relativePath}\" は対象プラットフォームが間違っているか、.NET アセンブリではありません。" +
                    $"BVE{otherBveVersion} 向けの AtsEX プラグインを BVE{currentBveVersion} で読み込もうとしているか、AtsEX プラグインではない可能性があります。",
                    senderFileName, lineIndex);
            }

            Type[] allTypes = assembly.GetTypes();
            IEnumerable<Type> pluginTypeCandidates = allTypes.Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && t.IsSubclassOf(typeof(AtsExPluginBase)));
            if (!pluginTypeCandidates.Any())
            {
                throw new BveFileLoadException(
                    $"\"{relativePath}\" で {nameof(AtsExPluginBase)} を継承しているクラスが見つかりませんでした。AtsEX プラグインではない可能性があります。" +
                    $"AtsEX プラグインとして認識されるには、{nameof(AtsExPluginBase)} を継承しているクラスが必要です。",
                    senderFileName, lineIndex);
            }

            IEnumerable<AtsExPluginInfo> plugins = pluginTypeCandidates.Select(t =>
            {
                ConstructorInfo constructorInfo = t.GetConstructor(new Type[] { typeof(HostServiceCollection) });
                if (constructorInfo is null) return null;

                AtsExPluginBase pluginInstance = constructorInfo.Invoke(new object[] { HostServiceCollection }) as AtsExPluginBase;
                if (pluginInstance.PluginType != pluginType) throw new ArgumentException($"{pluginType.GetTypeString()}として{pluginInstance.PluginType.GetTypeString()}を読み込もうとしました。");
                if (pluginInstance.PluginType == PluginType.MapPlugin && !pluginInstance.UseAtsExExtensions) throw new NotSupportedException($"{pluginInstance.PluginType.GetTypeString()}では AtsEX の拡張機能を使用しないプラグインを開発することはできません。");

                AtsExPluginInfo pluginInfo = new AtsExPluginInfo(assembly, t.FullName, pluginInstance);
                return pluginInfo;
            }).Where(plugin => !(plugin is null)).ToArray();
            if (!plugins.Any())
            {
                throw new BveFileLoadException(
                    $"\"{relativePath}\" で {nameof(AtsExPluginBase)} を継承しているクラスは見つかりましたが、パラメータ {typeof(HostServiceCollection)} を持つコンストラクタが見つかりませんでした。",
                    senderFileName, lineIndex);
            }

            return plugins;
        }
    }
}
