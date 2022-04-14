using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class MapObjectBase : ClassWrapper
    {
        static MapObjectBase()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<MapObjectBase>();

            LocationGetMethod = members.GetSourcePropertyGetterOf(nameof(Location));
            LocationSetMethod = members.GetSourcePropertySetterOf(nameof(Location));
        }

        protected MapObjectBase(object src) : base(src)
        {
        }

        public static MapObjectBase FromSource(object src)
        {
            if (src is null) return null;
            return new MapObjectBase(src);
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
            if (obj is MapObjectBase mapObject)
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
