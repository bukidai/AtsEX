using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal sealed class TimePosForm : ClassWrapper
    {
        static TimePosForm()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<TimePosForm>();

            SetScenarioMethod = members.GetSourceMethodOf(nameof(SetScenario));

            DrawMethod = members.GetSourceMethodOf(nameof(Draw));
        }

        private TimePosForm(object src) : base(src)
        {
        }

        public static TimePosForm FromSource(object src) => new TimePosForm(src);

        private static MethodInfo SetScenarioMethod;
        public void SetScenario(ScenarioProvider scenarioProvider) => SetScenarioMethod.Invoke(Src, new object[] { scenarioProvider.Src });

        private static MethodInfo DrawMethod;
        public void Draw() => DrawMethod.Invoke(Src, null);
    }
}
