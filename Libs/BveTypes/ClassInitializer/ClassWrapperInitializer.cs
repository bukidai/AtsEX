using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

namespace BveTypes
{
    internal sealed class ClassWrapperInitializer : ClassInitializerBase
    {
        private static BveTypeSet BveTypes;

        public static BveTypeSet LazyInitialize() => BveTypes;

        public ClassWrapperInitializer(BveTypeSet bveTypes) : base()
        {
            BveTypes = BveTypes ?? bveTypes;
        }

        public override void InitializeAll()
        {
            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
            IEnumerable<Type> classWrapperTypes = allTypes.Where(t => t == typeof(ClassWrapperBase) || t.IsSubclassOf(typeof(ClassWrapperBase)));

            Initialize<InitializeClassWrapperAttribute>(classWrapperTypes, method =>
            {
                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(BveTypeSet);
            }, new object[] { BveTypes });
        }
    }
}
