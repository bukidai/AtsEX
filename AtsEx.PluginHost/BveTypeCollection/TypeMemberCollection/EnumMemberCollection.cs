using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    public class EnumMemberCollection : TypeMemberCollectionBase
    {
        public EnumMemberCollection(Type wrapperType, Type originalType) : base(wrapperType, originalType)
        {
        }
    }
}
