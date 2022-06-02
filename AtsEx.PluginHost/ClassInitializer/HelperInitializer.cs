using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Helpers
{
    public sealed class HelperInitializer : ClassInitializerBase
    {
        public HelperInitializer(IApp app, IBveHacker bveHacker) : base(app, bveHacker)
        {
        }

        public override void InitializeAll()
        {
            Type[] allTypes = App.AtsExPluginHostAssembly.GetTypes();
            IEnumerable<Type> helperTypes = allTypes.Where(t => t.Namespace == typeof(HelperInitializer).Namespace);

            Initialize<InitializeHelperAttribute>(helperTypes, method =>
            {
                ParameterInfo[] paramters = method.GetParameters();
                if (paramters.Length != 2) return false;

                return paramters[0].ParameterType == typeof(IApp) && paramters[1].ParameterType == typeof(IBveHacker);
            }, Parameters);
        }
    }
}
