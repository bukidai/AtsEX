using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Plugins.Scripting;
using AtsEx.PluginHost;
using AtsEx.PluginHost.LoadErrorManager;

namespace AtsEx.Plugins
{
    internal partial class PluginLoader
    {
        private class PluginLoadErrorQueue
        {
            private readonly PluginLoadErrorResolver LoadErrorManager;
            private readonly Queue<Exception> ExceptionsToResolve = new Queue<Exception>();

            public PluginLoadErrorQueue(ILoadErrorManager loadErrorManager)
            {
                LoadErrorManager = new PluginLoadErrorResolver(loadErrorManager);
            }

            public void OnFailedToLoadAssembly(Assembly assembly, Exception ex)
            {
                Version pluginHostVersion = App.Instance.AtsExPluginHostAssembly.GetName().Version;
                Version referencedPluginHostVersion = assembly.GetReferencedPluginHost().Version;
                if (pluginHostVersion != referencedPluginHostVersion)
                {
                    string assemblyFileName = Path.GetFileName(assembly.Location);

                    string message = string.Format(Resources.Value.MaybeBecauseBuiltForDifferentVersion.Value, pluginHostVersion, App.Instance.ProductShortName);
                    BveFileLoadException additionalInfoException = new BveFileLoadException(message, assemblyFileName);

                    ExceptionsToResolve.Enqueue(additionalInfoException);
                }

                ExceptionsToResolve.Enqueue(ex);
            }

            public void OnFailedToLoadScriptPlugin(ScriptPluginPackage scriptPluginPackage, Exception ex)
            {
                ExceptionsToResolve.Enqueue(ex);
            }

            public void Resolve()
            {
                while (ExceptionsToResolve.Count > 0)
                {
                    Exception exception = ExceptionsToResolve.Dequeue();
                    try
                    {
                        LoadErrorManager.Resolve(exception);
                    }
                    catch
                    {
                        throw exception;
                    }
                }
            }
        }
    }
}
