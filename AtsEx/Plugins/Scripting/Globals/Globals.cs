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
        public IApp App { get; } = Automatic9045.AtsEx.App.Instance;
        public PluginHost.BveHacker BveHacker { get; }

        protected SortedList<string, dynamic> Variables;

        private Globals(PluginHost.BveHacker bveHacker, SortedList<string, dynamic> variables)
        {
            BveHacker = bveHacker;

            Variables = variables;
        }

        protected Globals(Globals source) : this(source.BveHacker, source.Variables)
        {
        }

        public Globals(PluginHost.BveHacker bveHacker) : this(bveHacker, new SortedList<string, dynamic>())
        {
        }

        public T GetVariable<T>(string name) => (T)Variables[name];

        public void SetVariable<T>(string name, T value) => Variables[name] = value;
    }
}
