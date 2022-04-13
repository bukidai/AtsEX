using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class StationList : MapObjectList
    {
        static StationList()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<StationList>();

            InsertMethod = members.GetSourceMethodOf(nameof(Insert));
            GetStandardTimeMethod = members.GetSourceMethodOf(nameof(GetStandardTime));
        }

        private StationList(object src) : base(src)
        {
        }

        public static new StationList FromSource(object src) => new StationList(src);

        private static MethodInfo InsertMethod;
        public void Insert(Station item) => InsertMethod.Invoke(Src, new object[] { item.Src });

        private static MethodInfo GetStandardTimeMethod;
        public int GetStandardTime(double location) => GetStandardTimeMethod.Invoke(Src, new object[] { location });
    }
}
