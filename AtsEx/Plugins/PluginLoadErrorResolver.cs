using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using UnembeddedResources;

using AtsEx.PluginHost;
using AtsEx.Scripting.CSharp;

namespace AtsEx.Plugins
{
    internal class PluginLoadErrorResolver : ILoadErrorResolver
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<PluginLoadErrorResolver>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> UnhandledExceptionCaption { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static PluginLoadErrorResolver()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private readonly LoadErrorManager LoadErrorManager;

        public PluginLoadErrorResolver(LoadErrorManager loadErrorManager)
        {
            LoadErrorManager = loadErrorManager;
        }

        public void Resolve(Exception exception)
        {
            if (exception is AggregateException ae)
            {
                foreach (Exception ex in ae.InnerExceptions)
                {
                    Resolve(ex);
                }
                return;
            }

            if (exception is CompilationException ce)
            {
                ce.ThrowAsLoadError(LoadErrorManager);
            }
            else if (exception is BveFileLoadException fe)
            {
                LoadErrorManager.Throw(fe.Message, fe.SenderFileName, fe.LineIndex, fe.CharIndex);
            }
            else
            {
                LoadErrorManager.Throw(exception.Message);
                MessageBox.Show(exception.ToString(), string.Format(Resources.Value.UnhandledExceptionCaption.Value, App.Instance.ProductShortName));
            }
        }
    }
}
