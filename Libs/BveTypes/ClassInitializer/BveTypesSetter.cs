using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BveTypes
{
    internal sealed class BveTypesSetter : ClassInitializerBase
    {
        private readonly BveTypeSet BveTypes;

        public BveTypesSetter(BveTypeSet bveTypes) : base()
        {
            BveTypes = bveTypes;
        }

        public override void InitializeAll()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            Initialize<SetBveTypesAttribute>(types, method =>
            {
                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(BveTypeSet);
            }, new object[] { BveTypes });
        }
    }
}
