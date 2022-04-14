using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    [UnderConstruction]
    public class BveFile : ClassWrapper
    {
        static BveFile()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<BveFile>();

            PathGetMethod = members.GetSourcePropertyGetterOf(nameof(Path));
            PathSetMethod = members.GetSourcePropertySetterOf(nameof(Path));
        }

        protected BveFile(object src) : base(src)
        {
        }

        public static BveFile FromSource(object src)
        {
            if (src is null) return null;
            return new BveFile(src);
        }

        private static MethodInfo PathGetMethod;
        private static MethodInfo PathSetMethod;
        public string Path
        {
            get => PathGetMethod.Invoke(Src, null);
            internal set => PathSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
