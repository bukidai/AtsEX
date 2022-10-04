using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    public class EnumMemberSet : TypeMemberSetBase
    {
        public EnumMemberSet(Type wrapperType, Type originalType) : base(wrapperType, originalType)
        {
        }
    }
}
