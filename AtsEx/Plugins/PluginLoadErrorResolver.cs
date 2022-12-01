using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using UnembeddedResources;

using AtsEx.PluginHost;
using AtsEx.PluginHost.LoadErrorManager;
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

        private readonly ILoadErrorManager LoadErrorManager;

        public PluginLoadErrorResolver(ILoadErrorManager loadErrorManager)
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
                foreach (Diagnostic error in ce.CompilationErrors)
                {
                    LinePosition linePosition = error.Location.GetLineSpan().StartLinePosition;
                    LoadErrorManager.Throw(error.GetMessage(System.Globalization.CultureInfo.CurrentUICulture), ce.SenderName, linePosition.Line, linePosition.Character + 1);
                }
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
