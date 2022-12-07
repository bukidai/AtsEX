using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.Plugins;
using AtsEx.PluginHost.MapStatements;
using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

namespace AtsEx
{
    internal partial class ScenarioService
    {
        private class PluginLoader
        {
            private readonly NativeImpl Native;
            private readonly BveHacker BveHacker;
            private readonly IExtensionSet Extensions;

            public PluginLoader(NativeImpl native, BveHacker bveHacker, IExtensionSet extensions)
            {
                Native = native;
                BveHacker = bveHacker;
                Extensions = extensions;
            }

            public PluginSet Load(PluginSourceSet vehiclePluginUsing)
            {
                PluginLoadErrorResolver loadErrorResolver = new PluginLoadErrorResolver(BveHacker.LoadErrorManager);

                Plugins.PluginLoader pluginLoader = new Plugins.PluginLoader(Native, BveHacker, Extensions);
                Dictionary<string, PluginBase> vehiclePlugins = null;
                Dictionary<string, PluginBase> mapPlugins = null;

                PluginSet loadedPlugins;
                try
                {
                    {
                        vehiclePlugins = pluginLoader.Load(vehiclePluginUsing);
                    }

                    {
                        PluginHost.MapStatements.Identifier mapPluginUsingIdentifier = new PluginHost.MapStatements.Identifier(Namespace.Root, "mappluginusing");
                        IEnumerable<IHeader> mapPluginUsingHeaders = BveHacker.MapHeaders.GetAll(mapPluginUsingIdentifier);

                        foreach (IHeader header in mapPluginUsingHeaders)
                        {
                            string mapPluginUsingPath = Path.Combine(Path.GetDirectoryName(BveHacker.ScenarioInfo.RouteFiles.SelectedFile.Path), header.Argument);
                            PluginSourceSet mapPluginUsing = PluginSourceSet.FromPluginUsing(PluginType.MapPlugin, mapPluginUsingPath);

                            Dictionary<string, PluginBase> loadedMapPlugins = pluginLoader.Load(mapPluginUsing);
                            AddRangeToMapPlugins(loadedMapPlugins);
                        }


                        void AddRangeToMapPlugins(Dictionary<string, PluginBase> plugins)
                        {
                            if (mapPlugins is null)
                            {
                                mapPlugins = plugins;
                            }
                            else
                            {
                                foreach (KeyValuePair<string, PluginBase> plugin in plugins)
                                {
                                    mapPlugins.Add(plugin.Key, plugin.Value);
                                }
                            }
                        }
                    }

                    {
                        IEnumerable<LoadError> removeTargetErrors = BveHacker.LoadErrorManager.Errors.Where(error =>
                        {
                            bool isTargetError = BveHacker._MapHeaders.Any(header => header.LineIndex == error.LineIndex && header.CharIndex == header.CharIndex);
                            return isTargetError;
                        });
                        foreach (LoadError error in removeTargetErrors)
                        {
                            BveHacker.LoadErrorManager.Errors.Remove(error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    loadErrorResolver.Resolve(ex);
                }
                finally
                {
                    if (vehiclePlugins is null) vehiclePlugins = new Dictionary<string, PluginBase>();
                    if (mapPlugins is null) mapPlugins = new Dictionary<string, PluginBase>();
                }

                loadedPlugins = new PluginSet(vehiclePlugins, mapPlugins);
                pluginLoader.SetPluginSetToLoadedPlugins(loadedPlugins);

                return loadedPlugins;
            }
        }
    }
}
