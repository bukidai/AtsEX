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
    [UnderConstruction]
    internal class BveFile : ClassWrapper, IBveFile
    {
        public BveFile(object src) : base(src)
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IBveFile>();

            PathGetMethod = members.GetSourcePropertyGetterOf(nameof(Path));
            PathSetMethod = members.GetSourcePropertySetterOf(nameof(Path));
        }

        protected MethodInfo PathGetMethod;
        protected MethodInfo PathSetMethod;
        public string Path
        {
            get => PathGetMethod.Invoke(Src, null);
            internal set => PathSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
