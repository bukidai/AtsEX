using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class UserVehicleLocationManager : LocationManager
    {
        static UserVehicleLocationManager()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<UserVehicleLocationManager>();

            LocationGetMethod = members.GetSourcePropertyGetterOf(nameof(Location));

            SetLocationMethod = members.GetSourceMethodOf(nameof(SetLocation));
        }

        protected UserVehicleLocationManager(object src) : base(src)
        {
        }

        public static new UserVehicleLocationManager FromSource(object src)
        {
            if (src is null) return null;
            return new UserVehicleLocationManager(src);
        }

        private static MethodInfo LocationGetMethod;
        public double Location
        {
            get => LocationGetMethod.Invoke(Src, null);
        }


        private static MethodInfo SetLocationMethod;
        public void SetLocation(double location, bool arg1) => SetLocationMethod.Invoke(Src, new object[] { location, arg1 });
    }
}
