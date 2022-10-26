using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.BveTypes;

namespace AtsEx.PluginHost.ClassWrappers
{
    internal sealed class ClassWrapperInitializer : ClassInitializerBase
    {
        private static BveHacker BveHacker = null;

        public static BveTypeSet LazyInitialize() => BveHacker.BveTypes;

        public ClassWrapperInitializer(BveHacker bveHacker) : base()
        {
            BveHacker = BveHacker ?? bveHacker;
        }

        public override void InitializeAll()
        {
            Type[] allTypes = App.Instance.AtsExPluginHostAssembly.GetTypes();
            IEnumerable<Type> classWrapperTypes = allTypes.Where(t => t == typeof(ClassWrapperBase) || t.IsSubclassOf(typeof(ClassWrapperBase)));

            Initialize<InitializeClassWrapperAttribute>(classWrapperTypes, method =>
            {
                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(BveTypeSet);
            }, new object[] { BveHacker.BveTypes });
        }
    }
}
