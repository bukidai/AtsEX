using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    public struct TextPosition : IComparable<TextPosition>
    {
        public int Line { get; }
        public int Character { get; }

        public TextPosition(int line, int character)
        {
            Line = line;
            Character = character;
        }

        public int CompareTo(TextPosition other) => Line == other.Line ? Character - other.Character : Line - other.Line;
    }
}
