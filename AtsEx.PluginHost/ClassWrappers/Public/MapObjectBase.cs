using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class MapObjectBase : ClassWrapper, IComparable
    {
        static MapObjectBase()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<MapObjectBase>();

            Constructor = members.GetSourceConstructor();

            LocationGetMethod = members.GetSourcePropertyGetterOf(nameof(Location));
            LocationSetMethod = members.GetSourcePropertySetterOf(nameof(Location));
        }

        protected MapObjectBase(object src) : base(src)
        {
        }

        private static ConstructorInfo Constructor;
        protected MapObjectBase(double location) : this(Constructor.Invoke(new object[] { location }))
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
