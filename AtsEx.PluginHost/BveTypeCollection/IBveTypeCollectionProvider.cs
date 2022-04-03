using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    public interface IBveTypeCollectionProvider
    {
        IBveTypeMemberCollection GetTypeInfoOf<TWrapper>();
        IBveTypeMemberCollection GetTypeInfoOf(Type wrapperType);
        Type GetWrapperTypeOf(Type originalType);
    }
}
