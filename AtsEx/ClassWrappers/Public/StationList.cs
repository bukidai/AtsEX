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
    internal sealed class StationList : MapObjectList, IStationList
    {
        static StationList()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IStationList>();

            InsertMethod = members.GetSourceMethodOf(nameof(Insert));
            GetStandardTimeMethod = members.GetSourceMethodOf(nameof(GetStandardTime));
        }

        public StationList(object src) : base(src)
        {
        }

        private static MethodInfo InsertMethod;
        public void Insert(IStation item) => InsertMethod.Invoke(Src, new object[] { item.Src });

        private static MethodInfo GetStandardTimeMethod;
        public int GetStandardTime(double location) => GetStandardTimeMethod.Invoke(Src, new object[] { location });
    }
}
