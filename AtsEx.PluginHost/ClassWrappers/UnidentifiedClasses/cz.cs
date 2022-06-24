using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    [WillRefactor]
    internal sealed class cz : ClassWrapperBase
    {
        public static class OriginalMemberSet
        {
            public static MethodInfo SetRouteMethod { get; internal set; }
        }

        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<cz>();

            OriginalMemberSet.SetRouteMethod = members.GetSourceMethodOf(nameof(SetRoute));
        }

        private cz(object src) : base(src)
        {
        }

        private void SetRoute(Route route)
        {
            throw new NotImplementedException();
        }
    }
}
