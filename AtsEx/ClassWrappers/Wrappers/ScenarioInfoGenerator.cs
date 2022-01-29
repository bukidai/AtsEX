using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal sealed class ScenarioInfoGenerator : IScenarioInfoGenerator
    {
        private BveHacker BveHacker;

        private Type ScenarioInfoType;
        private MethodInfo FromFileMethod;

        [WillRefactor]
        public ScenarioInfoGenerator(BveHacker bveHacker)
        {
            BveHacker = bveHacker;

            ScenarioInfoType = BveHacker.Assembly.GetType("ek");
            FromFileMethod = ScenarioInfoType.GetMethod("a", BindingFlags.Public | BindingFlags.Static);
        }

        public IScenarioInfo FromFile(string path)
        {
            dynamic src = FromFileMethod.Invoke(null, new object[] { path });
            return new ScenarioInfo(BveHacker.Assembly, src);
        }
    }
}
