using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    public partial class BveTypeSet
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<BveTypeSet>("PluginHost");

        protected SortedList<Type, TypeMemberSetBase> Types { get; }
        protected SortedList<Type, Type> OriginalAndWrapperTypes { get; }

        protected BveTypeSet(IEnumerable<TypeMemberSetBase> types, Type classWrapperType)
        {
            TypeMemberSetBase illegalType = types.FirstOrDefault(type => !(type.WrapperType.IsClass && type.WrapperType.IsSubclassOf(classWrapperType)) && !type.WrapperType.IsEnum);
            if (!(illegalType is null))
            {
                throw new ArgumentException(
                    string.Format(Resources.GetString("TypeNotClassWrapper").Value,
                    illegalType.WrapperType.FullName, classWrapperType.FullName));
            }

            Types = new SortedList<Type, TypeMemberSetBase>(types.ToDictionary(type => type.WrapperType, type => type), new TypeComparer());
            OriginalAndWrapperTypes = new SortedList<Type, Type>(types.ToDictionary(type => type.OriginalType, type => type.WrapperType), new TypeComparer());
        }

        public TypeMemberSetBase GetTypeInfoOf<TWrapper>() => Types[typeof(TWrapper)];
        public TypeMemberSetBase GetTypeInfoOf(Type wrapperType) => Types[wrapperType];
        public EnumMemberSet GetEnumInfoOf<TWrapper>() => (EnumMemberSet)Types[typeof(TWrapper)];
        public EnumMemberSet GetEnumInfoOf(Type wrapperType) => (EnumMemberSet)Types[wrapperType];
        public ClassMemberSet GetClassInfoOf<TWrapper>() => (ClassMemberSet)Types[typeof(TWrapper)];
        public ClassMemberSet GetClassInfoOf(Type wrapperType) => (ClassMemberSet)Types[wrapperType];
        public Type GetWrapperTypeOf(Type originalType) => OriginalAndWrapperTypes[originalType];

        public void Dispose()
        {
            Instance = null;
        }
    }
}
