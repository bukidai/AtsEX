using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.MapStatements;

namespace AtsEx.MapStatements
{
    internal sealed class Header : IHeader
    {
        public Identifier Name { get; }
        public string Argument { get; }

        public string DefinedFilePath { get; }
        public int LineIndex { get; }
        public int CharIndex { get; }

        public Header(Identifier identifier, string argument, string definedFilePath, int lineIndex, int charIndex)
        {
            Name = identifier;
            Argument = argument;

            DefinedFilePath = definedFilePath;
            LineIndex = lineIndex;
            CharIndex = charIndex;
        }
    }
}
