using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.Helpers;

namespace Automatic9045.AtsEx.PluginHost
{
    internal static class StaticConstructorInvoker
    {
        public static void InvokeAll()
        {
            Type[] allTypes = InstanceStore.App.AtsExPluginHostAssembly.GetTypes();

            IEnumerable<Type> classWrapperTypes = allTypes.Where(t => t.IsSubclassOf(typeof(ClassWrapper)));
            Parallel.ForEach(classWrapperTypes, type =>
            {
                type.TypeInitializer?.Invoke(null, null);
            });

            IEnumerable<Type> helperTypes = allTypes.Where(t => t.Namespace == typeof(ContextMenuHacker).Namespace);
            Parallel.ForEach(helperTypes, type =>
            {
                type.TypeInitializer?.Invoke(null, null);
            });
        }
    }
}
