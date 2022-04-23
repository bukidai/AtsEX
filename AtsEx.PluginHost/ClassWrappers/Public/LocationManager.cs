using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class LocationManager : ClassWrapper
    {
        static LocationManager()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<LocationManager>();

            SpeedMeterPerSecondGetMethod = members.GetSourcePropertyGetterOf(nameof(SpeedMeterPerSecond));

            SetSpeedMethod = members.GetSourceMethodOf(nameof(SetSpeed));
        }

        protected LocationManager(object src) : base(src)
        {
        }

        public static LocationManager FromSource(object src)
        {
            if (src is null) return null;
            return new LocationManager(src);
        }

        private static MethodInfo SpeedMeterPerSecondGetMethod;
        public double SpeedMeterPerSecond
        {
            get => SpeedMeterPerSecondGetMethod.Invoke(Src, null);
        }


        private static MethodInfo SetSpeedMethod;
        public void SetSpeed(double speedMeterPerSecond) => SetSpeedMethod.Invoke(Src, new object[] { speedMeterPerSecond });
    }
}
