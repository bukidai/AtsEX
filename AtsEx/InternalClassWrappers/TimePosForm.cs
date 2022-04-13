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
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<TimePosForm>();

            DrawMethod = members.GetSourceMethodOf(nameof(TimePosForm.Draw));
        }

        public TimePosForm(object src) : base(src)
        {
        }

        public void SetScenario(ScenarioProvider scenarioProvider) => throw new NotImplementedException();

        private static MethodInfo DrawMethod;
        public void Draw() => DrawMethod.Invoke(Src, null);
    }
}
