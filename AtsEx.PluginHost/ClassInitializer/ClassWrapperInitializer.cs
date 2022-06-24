using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class ClassWrapperInitializer : ClassInitializerBase
    {
        public ClassWrapperInitializer(IApp app, IBveHacker bveHacker) : base(app, bveHacker)
        {
        }

        public override void InitializeAll()
        {
            Type[] allTypes = App.AtsExPluginHostAssembly.GetTypes();
            IEnumerable<Type> classWrapperTypes = allTypes.Where(t => t.IsSubclassOf(typeof(ClassWrapperBase)));

            Initialize<InitializeClassWrapperAttribute>(classWrapperTypes, method =>
            {
                return method.GetParameters().Length == 0;
            }, null);
        }
    }
}
