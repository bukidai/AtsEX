using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    internal class TypeInfo
    {
        public Type WrapperType { get; }
        public Type OriginalType { get; }

        public TypeInfo Parent { get; }
        public IEnumerable<TypeInfo> Children { get; }

        public TypeMemberNameCollectionBase Src { get; }

        public TypeInfo(Type wrapperType, Type originalType, TypeInfo parent, IEnumerable<TypeInfo> children, TypeMemberNameCollectionBase src)
        {
            WrapperType = wrapperType;
            OriginalType = originalType;

            Parent = parent;
            Children = children;

            Src = src;
        }
    }
}
