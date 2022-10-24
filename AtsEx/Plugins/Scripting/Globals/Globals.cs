using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost;

namespace AtsEx.Plugins.Scripting
{
    public class Globals
    {
        public IApp App { get; } = global::AtsEx.App.Instance;
        public IScenarioService ScenarioService { get; }
        public PluginHost.BveHacker BveHacker { get; }

        protected SortedList<string, dynamic> Variables;

        private Globals(IScenarioService scenarioService, PluginHost.BveHacker bveHacker, SortedList<string, dynamic> variables)
        {
            ScenarioService = scenarioService;
            BveHacker = bveHacker;

            Variables = variables;
        }

        protected Globals(Globals source) : this(source.ScenarioService, source.BveHacker, source.Variables)
        {
        }

        public Globals(IScenarioService scenarioService, PluginHost.BveHacker bveHacker) : this(scenarioService, bveHacker, new SortedList<string, dynamic>())
        {
        }

        public T GetVariable<T>(string name) => (T)Variables[name];

        public void SetVariable<T>(string name, T value) => Variables[name] = value;
    }
}
