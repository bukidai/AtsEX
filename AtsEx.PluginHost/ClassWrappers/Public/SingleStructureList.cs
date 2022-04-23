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
    public sealed class SingleStructureList : MapObjectList
    {
        static SingleStructureList()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<SingleStructureList>();
        }

        private SingleStructureList(object src) : base(src)
        {
        }

        public static new SingleStructureList FromSource(object src)
        {
            if (src is null) return null;
            return new SingleStructureList(src);
        }
    }
}
