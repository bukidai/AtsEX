using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    public abstract class TypeMemberCollectionBase
    {
        public Type WrapperType { get; }
        public Type OriginalType { get; }

        internal TypeMemberCollectionBase(Type wrapperType, Type originalType)
        {
            WrapperType = wrapperType;
            OriginalType = originalType;
        }
    }
}
