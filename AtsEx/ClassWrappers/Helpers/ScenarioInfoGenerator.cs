using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal sealed class ScenarioInfoGenerator : IScenarioInfoGenerator
    {
        static ScenarioInfoGenerator()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IScenarioInfo>();

            FromFileMethod = members.GetSourceMethodOf(nameof(FromFile));
        }

        private static MethodInfo FromFileMethod;
        public IScenarioInfo FromFile(string path)
        {
            object src = FromFileMethod.Invoke(null, new object[] { path });
            return new ScenarioInfo(src);
        }
    }
}
