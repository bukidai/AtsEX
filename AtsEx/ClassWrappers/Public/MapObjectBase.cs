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
    internal class MapObjectBase : ClassWrapper, IMapObjectBase
    {
        static MapObjectBase()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IMapObjectBase>();

            LocationGetMethod = members.GetSourcePropertyGetterOf(nameof(Location));
            LocationSetMethod = members.GetSourcePropertySetterOf(nameof(Location));
        }

        public MapObjectBase(object src) : base(src)
        {
        }

        private static MethodInfo LocationGetMethod;
        private static MethodInfo LocationSetMethod;
        public double Location
        {
            get => LocationGetMethod.Invoke(Src, null);
            set => LocationSetMethod.Invoke(Src, new object[] { value });
        }

        public int CompareTo(object obj)
        {
            if (obj is IMapObjectBase mapObject)
            {
                return (Src as IComparable).CompareTo(mapObject.Src);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
