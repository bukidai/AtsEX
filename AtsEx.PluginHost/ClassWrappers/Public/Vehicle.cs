using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class Vehicle : ClassWrapper
    {
        static Vehicle()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<Vehicle>();
        }

        private Vehicle(object src) : base(src)
        {
        }

        public static Vehicle FromSource(object src)
        {
            if (src is null) return null;
            return new Vehicle(src);
        }
    }
}
