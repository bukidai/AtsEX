using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    public abstract class TypeMemberSetBase
    {
        public Type WrapperType { get; }
        public Type OriginalType { get; }

        internal TypeMemberSetBase(Type wrapperType, Type originalType)
        {
            WrapperType = wrapperType;
            OriginalType = originalType;
        }
    }
}
