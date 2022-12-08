using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    internal partial class AtsEx
    {
        private class ExtensionLoader
        {
            private readonly BveHacker BveHacker;

            public ExtensionLoader(BveHacker bveHacker)
            {
                BveHacker = bveHacker;
            }

            public ExtensionSet Load()
            {
                Plugins.PluginLoader pluginLoader = new Plugins.PluginLoader(null, BveHacker, null);

                string extensionsDirectory = Path.Combine(Path.GetDirectoryName(App.Instance.AtsExAssembly.Location), "Extensions");
                Directory.CreateDirectory(extensionsDirectory);

                string pluginUsingPath = Path.Combine(extensionsDirectory, "PluginUsing.xml");
                PluginSourceSet fromPluginUsing = File.Exists(pluginUsingPath)
                    ? PluginSourceSet.FromPluginUsing(PluginType.Extension, pluginUsingPath) : PluginSourceSet.Empty(PluginType.Extension);
                PluginSourceSet fromDirectory = PluginSourceSet.FromDirectory(null, PluginType.Extension, extensionsDirectory);

                Queue<Exception> exceptionsToResolve = new Queue<Exception>();
                Dictionary<string, PluginBase> loadedExtensions = pluginLoader.Load(fromPluginUsing.Concat(null, fromDirectory),
                    OnFailedToLoadAssembly, OnFailedToLoadScriptPlugin, OnFailedToLoadScriptPlugin);

                ExtensionSet extensions = new ExtensionSet(loadedExtensions.Values);
                ResolveExtensionLoadErrors(exceptionsToResolve);
                pluginLoader.SetExtensionSetToLoadedPlugins(extensions);

                return extensions;


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

                void ResolveExtensionLoadErrors(Queue<Exception> exceptions)
                {
                    PluginLoadErrorResolver loadErrorResolver = new PluginLoadErrorResolver(BveHacker.LoadErrorManager);
                    try
                    {
                        while (exceptions.Count > 0)
                        {
                            Exception exception = exceptions.Dequeue();
                            loadErrorResolver.Resolve(exception);
                        }
                    }
                    catch
                    {
                        throw exceptions.Peek();
                    }
                }
            }
        }
    }
}
