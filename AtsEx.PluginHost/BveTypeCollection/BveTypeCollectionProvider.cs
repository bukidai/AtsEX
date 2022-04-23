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
        protected SortedList<Type, TypeMemberCollectionBase> Types { get; }
        protected SortedList<Type, Type> OriginalAndWrapperTypes { get; }

        protected BveTypeCollectionProvider(IEnumerable<TypeMemberCollectionBase> types, Type classWrapperType)
        {
            TypeMemberCollectionBase illegalType = types.FirstOrDefault(type => !(type.WrapperType.IsClass && type.WrapperType.IsSubclassOf(classWrapperType)) && !type.WrapperType.IsEnum);
            if (!(illegalType is null))
            {
                throw new ArgumentException($"型 '{illegalType.WrapperType.FullName}' は {classWrapperType.FullName} を継承したクラスではありません。");
            }

            Types = new SortedList<Type, TypeMemberCollectionBase>(types.ToDictionary(type => type.WrapperType, type => type), new TypeComparer());
            OriginalAndWrapperTypes = new SortedList<Type, Type>(types.ToDictionary(type => type.OriginalType, type => type.WrapperType), new TypeComparer());
        }

        public TypeMemberCollectionBase GetTypeInfoOf<TWrapper>() => Types[typeof(TWrapper)];
        public TypeMemberCollectionBase GetTypeInfoOf(Type wrapperType) => Types[wrapperType];
        public EnumMemberCollection GetEnumInfoOf<TWrapper>() => (EnumMemberCollection)Types[typeof(TWrapper)];
        public EnumMemberCollection GetEnumInfoOf(Type wrapperType) => (EnumMemberCollection)Types[wrapperType];
        public ClassMemberCollection GetClassInfoOf<TWrapper>() => (ClassMemberCollection)Types[typeof(TWrapper)];
        public ClassMemberCollection GetClassInfoOf(Type wrapperType) => (ClassMemberCollection)Types[wrapperType];
        public Type GetWrapperTypeOf(Type originalType) => OriginalAndWrapperTypes[originalType];

        public void Dispose()
        {
            Instance = null;
        }
    }
}
