using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using UnembeddedResources;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx.Plugins.Scripting.CSharp
{
    internal sealed class CompilationException : Exception
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<CompilationException>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> Message { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly ResourceSet Resources = new ResourceSet();

        public IEnumerable<Diagnostic> CompilationErrors { get; }
        public string SenderName { get; }

        public CompilationException(string senderName, IEnumerable<Diagnostic> compilationErrors) : base(Resources.Message.Value)
        {
            SenderName = senderName;
            CompilationErrors = compilationErrors;
        }

        public void ThrowAsLoadError(LoadErrorManager loadErrorManager)
        {
            foreach (Diagnostic error in CompilationErrors)
            {
                LinePosition linePosition = error.Location.GetLineSpan().StartLinePosition;
                loadErrorManager.Throw(error.GetMessage(System.Globalization.CultureInfo.CurrentUICulture), SenderName, linePosition.Line, linePosition.Character + 1);
            }
        }
    }
}
