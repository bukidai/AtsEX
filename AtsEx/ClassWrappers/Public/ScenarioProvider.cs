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
    public class ScenarioProvider : ClassWrapper, IScenarioProvider
    {
        static ScenarioProvider()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IScenarioProvider>();

            RouteGetMethod = members.GetSourcePropertyGetterOf(nameof(Route));
            VehicleGetMethod = members.GetSourcePropertyGetterOf(nameof(Vehicle));
        }

        public ScenarioProvider(object src) : base(src)
        {
        }

        protected static MethodInfo RouteGetMethod;
        public PluginHost.ClassWrappers.IRoute Route
        {
            get => new Route(RouteGetMethod.Invoke(Src, null));
        }

        protected static MethodInfo VehicleGetMethod;
        public PluginHost.ClassWrappers.IVehicle Vehicle
        {
            get => new Vehicle(VehicleGetMethod.Invoke(Src, null));
        }
    }
}
