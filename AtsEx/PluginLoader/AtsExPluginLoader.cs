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
    internal class AtsExPluginLoader
    {
        public App App { get; }
        public BveHacker BveHacker { get; }

        public Vehicle Vehicle { get; }
        public Route Route { get; }

        public HostServiceCollection HostServiceCollection { get; }

        public AssemblyResolver AssemblyResolver { get; }

        public AtsExPluginLoader(App app, BveHacker bveHacker, Vehicle vehicle, Route route, AssemblyResolver assemblyResolver)
        {
            App = app;
            BveHacker = bveHacker;

            Vehicle = vehicle;
            Route = route;

            HostServiceCollection = new HostServiceCollection(App, BveHacker, Vehicle, Route);

            AssemblyResolver = assemblyResolver;
        }

        public IEnumerable<AtsExPluginInfo> LoadFromList(PluginType pluginType, string listAbsolutePath)
        {
            if (!File.Exists(listAbsolutePath))
            {
                throw new BveFileLoadException($"ATSEx プラグインリスト \"{listAbsolutePath}\" が見つかりません。");
            }

            IEnumerable<AtsExPluginListLoader.RecognizedDll> dlls = AtsExPluginListLoader.LoadFrom(listAbsolutePath);

            string pluginListName = Path.GetFileName(listAbsolutePath);
            return dlls.Select(dll => Load(pluginType, dll.RelativePath, dll.AbsolutePath, pluginListName, dll.LineIndex)).SelectMany(x => x);
        }

        public IEnumerable<AtsExPluginInfo> Load(PluginType pluginType, string relativePath, string absolutePath, string senderFileName = null, int lineIndex = 0)
        {
            Assembly assembly = null;
            AssemblyResolver.Register(assembly);
            try
            {
                assembly = Assembly.LoadFrom(absolutePath);
            }
            catch (BadImageFormatException)
            {
                int currentBveVersion = App.BveInfo.Version.Major;
                int otherBveVersion = currentBveVersion == 6 ? 5 : 6;
                throw new BveFileLoadException(
                    $"\"{relativePath}\" は対象プラットフォームが間違っているか、.NET アセンブリではありません。" +
                    $"BVE{otherBveVersion} 向けの AtsEX プラグインを BVE{currentBveVersion} で読み込もうとしているか、AtsEX プラグインではない可能性があります。",
                    senderFileName, lineIndex);
            }

            Type[] allTypes = assembly.GetTypes();
            IEnumerable<Type> pluginTypeCandidates = allTypes.Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && t.IsSubclassOf(typeof(AtsExPlugin)));
            if (!pluginTypeCandidates.Any())
            {
                throw new BveFileLoadException(
                    $"\"{relativePath}\" で {nameof(AtsExPlugin)} を継承しているクラスが見つかりませんでした。AtsEX プラグインではない可能性があります。" +
                    $"AtsEX プラグインとして認識されるには、{nameof(AtsExPlugin)} を継承しているクラスが必要です。",
                    senderFileName, lineIndex);
            }

            IEnumerable<AtsExPluginInfo> plugins = pluginTypeCandidates.Select(t =>
            {
                ConstructorInfo constructorInfo = t.GetConstructor(new Type[] { typeof(IHostServiceCollection) });
                if (constructorInfo is null) return null;

                AtsExPlugin pluginInstance = constructorInfo.Invoke(new object[] { HostServiceCollection }) as AtsExPlugin;
                AtsExPluginInfo pluginInfo = new AtsExPluginInfo(pluginType, assembly, t.FullName, pluginInstance);
                return pluginInfo;
            }).Where(plugin => !(plugin is null)).ToArray();
            if (!plugins.Any())
            {
                throw new BveFileLoadException(
                    $"\"{relativePath}\" で {nameof(AtsExPlugin)} を継承しているクラスは見つかりましたが、引数の型が {nameof(IHostServiceCollection)} であるコンストラクタが見つかりませんでした。",
                    senderFileName, lineIndex);
            }

            return plugins;
        }
    }
}
