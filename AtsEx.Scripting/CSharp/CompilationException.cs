using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using UnembeddedResources;

using AtsEx.PluginHost;

namespace AtsEx.Scripting.CSharp
{
    public sealed class CompilationException : Exception
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

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static CompilationException()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        public IEnumerable<Diagnostic> CompilationErrors { get; }
        public string SenderName { get; }

        public CompilationException(string senderName, IEnumerable<Diagnostic> compilationErrors) : base(Resources.Value.Message.Value)
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
