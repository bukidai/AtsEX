using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    public partial class BveTypeCollectionProvider
    {
        protected SortedList<Type, BveTypeMemberCollection> Types { get; }
        protected SortedList<Type, Type> OriginalAndWrapperTypes { get; }

        protected BveTypeCollectionProvider(IEnumerable<BveTypeMemberCollection> types, Type classWrapperType)
        {
            BveTypeMemberCollection illegalType = types.FirstOrDefault(type => !type.WrapperType.IsClass || !type.WrapperType.IsSubclassOf(classWrapperType));
            if (!(illegalType is null))
            {
                throw new ArgumentException($"型 '{illegalType.WrapperType.FullName}' は {classWrapperType.FullName} を継承したクラスではありません。");
            }

            Types = new SortedList<Type, BveTypeMemberCollection>(types.ToDictionary(type => type.WrapperType, type => type), new TypeComparer());
            OriginalAndWrapperTypes = new SortedList<Type, Type>(types.ToDictionary(type => type.OriginalType, type => type.WrapperType), new TypeComparer());
        }

        public BveTypeMemberCollection GetTypeInfoOf<TWrapper>() => Types[typeof(TWrapper)];
        public BveTypeMemberCollection GetTypeInfoOf(Type wrapperType) => Types[wrapperType];
        public Type GetWrapperTypeOf(Type originalType) => OriginalAndWrapperTypes[originalType];

        public void Dispose()
        {
            Instance = null;
        }
    }
}
