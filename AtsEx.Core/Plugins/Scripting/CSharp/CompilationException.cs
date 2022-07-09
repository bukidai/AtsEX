using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.Plugins.Scripting.CSharp
{
    public class CompilationException : Exception
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<CompilationException>("Core");

        public IEnumerable<Diagnostic> CompilationErrors { get; }

        public CompilationException(IEnumerable<Diagnostic> compilationErrors) : base(Resources.GetString("Message").Value)
        {
            CompilationErrors = compilationErrors;
        }
    }
}
