using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    /// <summary>
    /// クラスラッパーに対応する BVE の型とメンバーの情報を提供します。
    /// </summary>
    public sealed partial class BveTypeSet
    {
        private partial class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<BveTypeSet>("PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> TypeNotClassWrapper { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly ResourceSet Resources = new ResourceSet();

        private readonly SortedList<Type, TypeMemberSetBase> Types;
        private readonly SortedList<Type, Type> OriginalAndWrapperTypes;

        private BveTypeSet(IEnumerable<TypeMemberSetBase> types, Version profileVersion)
        {
            TypeMemberSetBase illegalType = types.FirstOrDefault(type => !(type.WrapperType.IsClass && type.WrapperType.IsSubclassOf(typeof(ClassWrapperBase))) && !type.WrapperType.IsEnum);
            if (!(illegalType is null))
            {
                throw new ArgumentException(
                    string.Format(Resources.TypeNotClassWrapper.Value,
                    illegalType.WrapperType.FullName, typeof(ClassWrapperBase).FullName));
            }

            Types = new SortedList<Type, TypeMemberSetBase>(types.ToDictionary(type => type.WrapperType, type => type), new TypeComparer());
            OriginalAndWrapperTypes = new SortedList<Type, Type>(types.ToDictionary(type => type.OriginalType, type => type.WrapperType), new TypeComparer());

            ProfileVersion = profileVersion;
        }


        /// <summary>
        /// 読み込まれたプロファイルが対応している BVE のバージョンを取得します。
        /// </summary>
        public Version ProfileVersion { get; }


        /// <summary>
        /// <typeparamref name="TWrapper"/> に指定したラッパー型の情報を取得します。
        /// </summary>
        /// <typeparam name="TWrapper">ラッパー型。</typeparam>
        /// <returns><typeparamref name="TWrapper"/> に指定したラッパー型の情報を表す <see cref="TypeMemberSetBase"/>。</returns>
        /// <seealso cref="GetClassInfoOf{TWrapper}"/>
        /// <seealso cref="GetClassInfoOf(Type)"/>
        /// <seealso cref="GetEnumInfoOf{TWrapper}"/>
        /// <seealso cref="GetEnumInfoOf(Type)"/>
        public TypeMemberSetBase GetTypeInfoOf<TWrapper>() => Types[typeof(TWrapper)];

        /// <summary>
        /// <paramref name="wrapperType"/> に指定したラッパー型の情報を取得します。
        /// </summary>
        /// <param name="wrapperType">ラッパー型。</param>
        /// <returns><paramref name="wrapperType"/> に指定したラッパー型の情報を表す <see cref="TypeMemberSetBase"/>。</returns>
        /// <seealso cref="GetClassInfoOf{TWrapper}"/>
        /// <seealso cref="GetClassInfoOf(Type)"/>
        /// <seealso cref="GetEnumInfoOf{TWrapper}"/>
        /// <seealso cref="GetEnumInfoOf(Type)"/>
        public TypeMemberSetBase GetTypeInfoOf(Type wrapperType) => Types[wrapperType];


        /// <summary>
        /// <typeparamref name="TWrapper"/> に指定したラッパー列挙型の情報を取得します。
        /// </summary>
        /// <typeparam name="TWrapper">ラッパー列挙型。</typeparam>
        /// <returns><typeparamref name="TWrapper"/> に指定したラッパー列挙型の情報を表す <see cref="EnumMemberSet"/>。</returns>
        public EnumMemberSet GetEnumInfoOf<TWrapper>() => (EnumMemberSet)Types[typeof(TWrapper)];

        /// <summary>
        /// <paramref name="wrapperType"/> に指定したラッパー列挙型の情報を取得します。
        /// </summary>
        /// <param name="wrapperType">ラッパー列挙型。</param>
        /// <returns><paramref name="wrapperType"/> に指定したラッパー列挙型の情報を表す <see cref="EnumMemberSet"/>。</returns>
        public EnumMemberSet GetEnumInfoOf(Type wrapperType) => (EnumMemberSet)Types[wrapperType];


        /// <summary>
        /// <typeparamref name="TWrapper"/> に指定したラッパークラスの情報を取得します。
        /// </summary>
        /// <typeparam name="TWrapper">ラッパークラス。</typeparam>
        /// <returns><typeparamref name="TWrapper"/> に指定したラッパークラスの情報を表す <see cref="ClassMemberSet"/>。</returns>
        public ClassMemberSet GetClassInfoOf<TWrapper>() => (ClassMemberSet)Types[typeof(TWrapper)];

        /// <summary>
        /// <paramref name="wrapperType"/> に指定したラッパークラスの情報を取得します。
        /// </summary>
        /// <param name="wrapperType">ラッパークラス。</param>
        /// <returns><paramref name="wrapperType"/> に指定したラッパークラスの情報を表す <see cref="ClassMemberSet"/>。</returns>
        public ClassMemberSet GetClassInfoOf(Type wrapperType) => (ClassMemberSet)Types[wrapperType];


        /// <summary>
        /// <paramref name="originalType"/> に指定したオリジナル型のラッパー型を取得します。
        /// </summary>
        /// <param name="originalType">オリジナル型。</param>
        /// <returns></returns>
        public Type GetWrapperTypeOf(Type originalType) => OriginalAndWrapperTypes[originalType];
    }
}
