using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost;

namespace AtsEx.Scripting
{
    public class Globals
    {
        public IScenarioService ScenarioService { get; }
        public BveHacker BveHacker { get; }

        protected Dictionary<string, dynamic> Variables;

        private Globals(IScenarioService scenarioService, BveHacker bveHacker, Dictionary<string, dynamic> variables)
        {
            ScenarioService = scenarioService;
            BveHacker = bveHacker;

            Variables = variables;
        }

        protected Globals(Globals source) : this(source.ScenarioService, source.BveHacker, source.Variables)
        {
        }

        public Globals(IScenarioService scenarioService, BveHacker bveHacker) : this(scenarioService, bveHacker, new Dictionary<string, dynamic>())
        {
        }

        public T GetVariable<T>(string name) => (T)Variables[name];

        public void SetVariable<T>(string name, T value) => Variables[name] = value;
    }
}
