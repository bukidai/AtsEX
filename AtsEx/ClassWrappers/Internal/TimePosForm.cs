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
    internal interface ITimePosForm : IClassWrapper
    {
        void SetScenario(IScenarioProvider scenarioProvider);
        void Draw();
    }

    internal class TimePosForm : ClassWrapper, ITimePosForm
    {
        public TimePosForm(object src) : base(src)
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<ITimePosForm>();

            DrawMethod = members.GetSourceMethodOf(nameof(ITimePosForm.Draw));
        }

        public void SetScenario(IScenarioProvider scenarioProvider) => throw new NotImplementedException();

        protected MethodInfo DrawMethod;
        public void Draw() => DrawMethod.Invoke(Src, null);
    }
}
