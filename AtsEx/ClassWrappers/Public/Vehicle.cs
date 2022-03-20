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
    internal sealed class Vehicle : ClassWrapper, PluginHost.ClassWrappers.IVehicle
    {
        static Vehicle()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<PluginHost.ClassWrappers.IVehicle>();
        }

        public Vehicle(object src) : base(src)
        {
        }
    }
}
