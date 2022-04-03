using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.BveTypeCollection
{
    public partial class BveTypeCollectionProvider : IBveTypeCollectionProvider
    {
        protected SortedList<Type, IBveTypeMemberCollection> Types { get; }
        protected SortedList<Type, Type> OriginalAndWrapperTypes { get; }

        protected BveTypeCollectionProvider(IEnumerable<IBveTypeMemberCollection> types)
        {
            IBveTypeMemberCollection illegalType = types.FirstOrDefault(type => !type.WrapperType.IsInterface || !type.WrapperType.GetInterfaces().Contains(typeof(IClassWrapper)));
            if (!(illegalType is null))
            {
                throw new ArgumentException($"型 '{illegalType.WrapperType.FullName}' は {typeof(IClassWrapper).FullName} を継承したインターフェースではありません。");
            }

            Types = new SortedList<Type, IBveTypeMemberCollection>(types.ToDictionary(type => type.WrapperType, type => type), new TypeComparer());
            OriginalAndWrapperTypes = new SortedList<Type, Type>(types.ToDictionary(type => type.OriginalType, type => type.WrapperType), new TypeComparer());
        }

        public IBveTypeMemberCollection GetTypeInfoOf<TWrapper>() => Types[typeof(TWrapper)];
        public IBveTypeMemberCollection GetTypeInfoOf(Type wrapperType) => Types[wrapperType];
        public Type GetWrapperTypeOf(Type originalType) => OriginalAndWrapperTypes[originalType];

        public void Dispose()
        {
            Instance = null;
        }
    }
}
