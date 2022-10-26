using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.BveTypes;

namespace AtsEx.PluginHost
{
    internal sealed class BveTypesSetter : ClassInitializerBase
    {
        private readonly BveHacker BveHacker;

        public BveTypesSetter(BveHacker bveHacker) : base()
        {
            BveHacker = bveHacker;
        }

        public override void InitializeAll()
        {
            Type[] types = App.Instance.AtsExPluginHostAssembly.GetTypes();

            Initialize<SetBveTypesAttribute>(types, method =>
            {
                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(BveTypeSet);
            }, new object[] { BveHacker.BveTypes });
        }
    }
}
