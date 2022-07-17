using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx.Plugins.Scripting
{
    public class Globals
    {
        public IBveHacker BveHacker { get; }

        protected SortedList<string, dynamic> Variables;

        private Globals(IBveHacker bveHacker, SortedList<string, dynamic> variables)
        {
            BveHacker = bveHacker;

            Variables = variables;
        }

        protected Globals(Globals source) : this(source.BveHacker, source.Variables)
        {
        }

        public Globals(IBveHacker bveHacker) : this(bveHacker, new SortedList<string, dynamic>())
        {
        }

        public T GetVariable<T>(string name) => (T)Variables[name];

        public void SetVariable<T>(string name, T value) => Variables[name] = value;
    }
}
