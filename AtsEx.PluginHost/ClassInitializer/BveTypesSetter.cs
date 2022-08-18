using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost
{
    internal sealed class BveTypesSetter : ClassInitializerBase
    {
        public BveTypesSetter(IApp app, BveHacker bveHacker) : base(app, bveHacker)
        {
        }

        public override void InitializeAll()
        {
            Type[] types = App.AtsExPluginHostAssembly.GetTypes();

            Initialize<SetBveTypesAttribute>(types, method =>
            {
                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(BveTypeSet);
            }, new object[] { BveHacker.BveTypes });
        }
    }
}
