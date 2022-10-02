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
    public sealed partial class BveTypeSet
    {
        private partial class ResourceSet
        {
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PropertyWrapperNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PropertyGetterOriginalNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> InvalidPropertyTypeWrapper { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PropertySetterOriginalNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> FieldWrapperPropertyNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> FieldOriginalNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> MethodWrapperNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> MethodOriginalNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> CollectionTypeNotRecognized { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> OriginalTypeNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> GenericWrapperTypeNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> WrapperTypeNotFound { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> Public { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> NonPublic { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> Instance { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> Static { get; private set; }
        }

        private const BindingFlags SearchAllBindingAttribute = BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

        /// <summary>
        /// BVE のアセンブリとバージョンを指定して、クラスラッパーに対応する BVE の型とメンバーの定義を読み込みます。
        /// </summary>
        /// <remarks>
        /// このメソッドの実行には時間がかかります。特段の事情が無い限りは <see cref="BveHacker.BveTypes"/> プロパティを参照してください。
        /// </remarks>
        /// <param name="bveAssembly">BVE の <see cref="Assembly"/>。</param>
        /// <param name="bveVersion">BVE のバージョン。</param>
        /// <param name="allowLoadProfileForDifferentBveVersion">実行中の BVE がサポートされないバージョンの場合、他のバージョン向けのプロファイルで代用するか。</param>
        /// <param name="profileForDifferentBveVersionLoaded">実行中の BVE がサポートされないバージョンであり、他のバージョン向けのプロファイルで代用された時に実行するデリケート。パラメータにはプロファイルのバージョンが渡されます。</param>
        /// <returns><see cref="BveTypeSet"/> クラスの新しいインスタンス。</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static BveTypeSet Load(Assembly bveAssembly, Version bveVersion, bool allowLoadProfileForDifferentBveVersion, Action<Version> profileForDifferentBveVersionLoaded = null)
        {
            Assembly pluginHostAssembly = Assembly.GetExecutingAssembly();

            ProfileSelector profileSelector = new ProfileSelector(bveVersion);
            Version profileVersion;
            List<TypeMemberNameSetBase> typeMemberNames;
            using (Profile profile = profileSelector.GetProfileStream(allowLoadProfileForDifferentBveVersion))
            {
                profileVersion = profile.Version;

                if (profileVersion != bveVersion)
                {
                    profileForDifferentBveVersionLoaded?.Invoke(profileVersion);
                }

                using (Stream schema = profileSelector.GetSchemaStream())
                {
                    typeMemberNames = WrapTypesXmlLoader.LoadFile(profile.Stream, schema);
                }
            }

            IEnumerable<Type> wrapperTypes = pluginHostAssembly.GetTypes().Where(type => (type.IsClass && type.IsSubclassOf(typeof(ClassWrapperBase))) || type.IsEnum);
            IEnumerable<Type> originalTypes = bveAssembly.GetTypes();

            TypeInfoCreator typeInfoCreator = new TypeInfoCreator(bveAssembly);
            IEnumerable<TypeInfo> typeInfos = typeInfoCreator.Create(typeMemberNames);

            IEnumerable<TypeMemberSetBase> types = typeMemberNames.Select<TypeMemberNameSetBase, TypeMemberSetBase>(src =>
            {
                TypeInfo typeInfo = typeInfos.First(t => t.Src == src);

                Type wrapperType = typeInfo.WrapperType;
                Type originalType = typeInfo.OriginalType;

                switch (src)
                {
                    case EnumMemberNameSet enumSrc:
                    {
                        EnumMemberSet members = new EnumMemberSet(wrapperType, originalType);
                        return members;
                    }

                    case ClassMemberNameSet classSrc:
                    {
                        SortedList<Type[], ConstructorInfo> constructors = new SortedList<Type[], ConstructorInfo>(originalType.GetConstructors(SearchAllBindingAttribute).ToDictionary(
                            c => c.GetParameters().Select(p => GetWrapperTypeIfOriginal(p.ParameterType)).ToArray(),
                            c => c),
                            new TypeArrayComparer());

                        SortedList<string, MethodInfo> propertyGetters = new SortedList<string, MethodInfo>(classSrc.PropertyGetters.Select(getterInfo =>
                        {
                            PropertyInfo wrapperProperty = wrapperType.GetProperty(getterInfo.WrapperName, CreateBindingAttribute(getterInfo.IsWrapperPrivate, getterInfo.IsWrapperStatic));
                            if (wrapperProperty is null)
                            {
                                throw new KeyNotFoundException(
                                    string.Format(Resources.PropertyWrapperNotFound.Value,
                                    GetAccessibilityDescription(getterInfo.IsWrapperPrivate), getterInfo.WrapperName, classSrc.WrapperTypeName));
                            }

                            MethodInfo originalGetter = originalType.GetMethod(getterInfo.OriginalName, CreateBindingAttribute(getterInfo.IsOriginalPrivate, getterInfo.IsOriginalStatic), null, Type.EmptyTypes, null);
                            if (originalGetter is null)
                            {
                                throw new KeyNotFoundException(
                                    string.Format(Resources.PropertyGetterOriginalNotFound.Value,
                                    GetAccessibilityDescription(getterInfo.IsOriginalPrivate, getterInfo.IsOriginalStatic), getterInfo.OriginalName, classSrc.WrapperTypeName, classSrc.OriginalTypeName));
                            }

                            return new KeyValuePair<string, MethodInfo>(wrapperProperty.Name, originalGetter);
                        }).ToDictionary(x => x.Key, x => x.Value));

                        SortedList<string, MethodInfo> propertySetters = new SortedList<string, MethodInfo>(classSrc.PropertySetters.Select(setterInfo =>
                        {
                            PropertyInfo wrapperProperty = wrapperType.GetProperty(setterInfo.WrapperName, CreateBindingAttribute(setterInfo.IsWrapperPrivate, setterInfo.IsWrapperStatic));
                            if (wrapperProperty is null)
                            {
                                throw new KeyNotFoundException(
                                    string.Format(Resources.PropertyWrapperNotFound.Value,
                                    GetAccessibilityDescription(setterInfo.IsWrapperPrivate), setterInfo.WrapperName, classSrc.WrapperTypeName));
                            }

                            Type originalParamType = GetOriginalTypeIfWrapper(wrapperProperty.PropertyType,
                                string.Format(Resources.InvalidPropertyTypeWrapper.Value,
                                $"{classSrc.WrapperTypeName}.{setterInfo.WrapperName}", wrapperProperty.PropertyType.Name));
                            MethodInfo originalSetter = originalType.GetMethod(setterInfo.OriginalName, CreateBindingAttribute(setterInfo.IsOriginalPrivate, setterInfo.IsOriginalStatic), null, new Type[] { originalParamType }, null);
                            if (originalSetter is null)
                            {
                                throw new KeyNotFoundException(
                                    string.Format(Resources.PropertySetterOriginalNotFound.Value,
                                    GetAccessibilityDescription(setterInfo.IsOriginalPrivate, setterInfo.IsOriginalStatic), setterInfo.OriginalName, classSrc.WrapperTypeName, classSrc.OriginalTypeName));
                            }

                            return new KeyValuePair<string, MethodInfo>(wrapperProperty.Name, originalSetter);
                        }).ToDictionary(x => x.Key, x => x.Value));

                        SortedList<string, FieldInfo> fields = new SortedList<string, FieldInfo>(classSrc.Fields.Select(fieldInfo =>
                        {
                            PropertyInfo wrapperProperty = wrapperType.GetProperty(fieldInfo.WrapperName, CreateBindingAttribute(fieldInfo.IsWrapperPrivate, fieldInfo.IsWrapperStatic));
                            if (wrapperProperty is null)
                            {
                                throw new KeyNotFoundException(
                                    string.Format(Resources.FieldWrapperPropertyNotFound.Value,
                                    GetAccessibilityDescription(fieldInfo.IsWrapperPrivate), fieldInfo.WrapperName, classSrc.WrapperTypeName));
                            }

                            FieldInfo originalField = originalType.GetField(fieldInfo.OriginalName, CreateBindingAttribute(fieldInfo.IsOriginalPrivate, fieldInfo.IsOriginalStatic));
                            if (originalField is null)
                            {
                                throw new KeyNotFoundException(
                                    string.Format(Resources.FieldOriginalNotFound.Value,
                                    GetAccessibilityDescription(fieldInfo.IsOriginalPrivate, fieldInfo.IsOriginalStatic), fieldInfo.OriginalName, classSrc.WrapperTypeName, classSrc.OriginalTypeName));
                            }

                            return new KeyValuePair<string, FieldInfo>(wrapperProperty.Name, originalField);
                        }).ToDictionary(x => x.Key, x => x.Value));

                        SortedList<(string, Type[]), MethodInfo> methods = new SortedList<(string, Type[]), MethodInfo>(classSrc.Methods.Select(methodInfo =>
                        {
                            Type[] wrapperMethodParams = methodInfo.WrapperParamNames.Select(p => ParseTypeName(p, false)).ToArray();
                            MethodInfo wrapperMethod = wrapperType.GetMethod(methodInfo.WrapperName, CreateBindingAttribute(methodInfo.IsWrapperPrivate, methodInfo.IsWrapperStatic), null, wrapperMethodParams, null);
                            if (wrapperMethod is null)
                            {
                                throw new KeyNotFoundException(
                                    string.Format(Resources.MethodWrapperNotFound.Value,
                                    string.Join(", ", wrapperMethodParams.Select(GetTypeFullName)), GetAccessibilityDescription(methodInfo.IsWrapperPrivate), methodInfo.WrapperName, classSrc.WrapperTypeName));
                            }

                            Type[] originalMethodParams = methodInfo.WrapperParamNames.Select(p => ParseTypeName(p, true)).ToArray();
                            MethodInfo originalMethod = originalType.GetMethod(methodInfo.OriginalName, CreateBindingAttribute(methodInfo.IsOriginalPrivate, methodInfo.IsOriginalStatic), null, originalMethodParams, null);
                            if (originalMethod is null)
                            {
                                throw new KeyNotFoundException(
                                    string.Format(Resources.MethodOriginalNotFound.Value,
                                    string.Join(", ", originalMethodParams.Select(GetTypeFullName)), GetAccessibilityDescription(methodInfo.IsOriginalPrivate, methodInfo.IsOriginalStatic), methodInfo.OriginalName, classSrc.WrapperTypeName, classSrc.OriginalTypeName));
                            }

                            return new KeyValuePair<(string, Type[]), MethodInfo>((wrapperMethod.Name, wrapperMethodParams), originalMethod);
                        }).ToDictionary(x => x.Key, x => x.Value), new StringTypeArrayTupleComparer());


                        ClassMemberSet members = new ClassMemberSet(wrapperType, originalType, constructors, propertyGetters, propertySetters, fields, methods);
                        return members;
                    }

                    default:
                        throw new NotImplementedException(string.Format(Resources.CollectionTypeNotRecognized.Value, nameof(TypeMemberNameSetBase), src.GetType().Name));

                }
            });

            return new BveTypeSet(types, profileVersion);


            #region メソッド
            Type GetOriginalTypeIfWrapper(Type type, string typeLoadExceptionMessage = null)
            {
                if (type.IsArray)
                {
                    int arrayRank = type.GetArrayRank();
                    Type elementType = type.GetElementType();
                    Type originalElementType = GetOriginalTypeIfWrapper(elementType, typeLoadExceptionMessage);
                    type = arrayRank == 1 ? originalElementType.MakeArrayType() : originalElementType.MakeArrayType(arrayRank);
                }
                else if (type.IsConstructedGenericType)
                {
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    Type[] typeParams = type.GetGenericArguments().Select(t => GetOriginalTypeIfWrapper(t, typeLoadExceptionMessage)).ToArray();

                    if (genericTypeDefinition == typeof(WrappedList<>))
                    {
                        genericTypeDefinition = typeof(List<>);
                    }
                    else if (genericTypeDefinition == typeof(WrappedSortedList<,>))
                    {
                        genericTypeDefinition = typeof(SortedList<,>);
                    }

                    type = genericTypeDefinition.MakeGenericType(typeParams);
                }

                if (type.IsClass && type.IsSubclassOf(typeof(ClassWrapperBase)))
                {
                    Type originalType = typeInfos.FirstOrDefault(x => x.WrapperType == type)?.OriginalType;
                    if (originalType is null)
                    {
                        if (typeLoadExceptionMessage is null)
                        {
                            typeLoadExceptionMessage = string.Format(Resources.OriginalTypeNotFound.Value, type.Name);
                        }

                        throw new TypeLoadException(typeLoadExceptionMessage);
                    }

                    return originalType;
                }
                else if (type.IsEnum)
                {
                    Type originalType = typeInfos.FirstOrDefault(x => x.WrapperType == type)?.OriginalType;
                    return originalType ?? type;
                }
                else
                {
                    return type;
                }
            }

            Type GetWrapperTypeIfOriginal(Type type, string typeLoadExceptionMessage = null)
            {
                if (type.IsArray)
                {
                    int arrayRank = type.GetArrayRank();
                    Type elementType = type.GetElementType();
                    Type wrapperElementType = GetWrapperTypeIfOriginal(elementType, typeLoadExceptionMessage);
                    type = arrayRank == 1 ? wrapperElementType.MakeArrayType() : wrapperElementType.MakeArrayType(arrayRank);
                }
                else if (type.IsConstructedGenericType)
                {
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    Type[] typeParams = type.GetGenericArguments().Select(t => GetWrapperTypeIfOriginal(t, typeLoadExceptionMessage)).ToArray();

                    if (genericTypeDefinition == typeof(List<>))
                    {
                        genericTypeDefinition = typeof(WrappedList<>);
                    }
                    else if (genericTypeDefinition == typeof(SortedList<,>))
                    {
                        genericTypeDefinition = typeof(WrappedSortedList<,>);
                    }

                    type = genericTypeDefinition.MakeGenericType(typeParams);
                }

                Type wrapperType = typeInfos.FirstOrDefault(x => x.OriginalType == type)?.WrapperType;
                return wrapperType ?? type;
            }

            Type ParseTypeName(TypeMemberNameSetBase.TypeInfoBase typeInfo, bool convertToOriginalType)
            {
                Type type;
                switch (typeInfo)
                {
                    case TypeMemberNameSetBase.ArrayTypeInfo arrayTypeInfo:
                    {
                        Type baseType = ParseTypeName(arrayTypeInfo.BaseType, convertToOriginalType);
                        type = baseType.MakeArrayType(arrayTypeInfo.DimensionCount);

                        break;
                    }

                    case TypeMemberNameSetBase.GenericTypeInfo genericTypeInfo:
                    {
                        Type[] paramTypes = genericTypeInfo.TypeParams.Select(t => ParseTypeName(t, convertToOriginalType)).ToArray();

                        Type genericDefinitionType = TryGetType($"{genericTypeInfo.TypeName}`{paramTypes.Length}");
                        if (!(genericDefinitionType is null))
                        {
                            type = genericDefinitionType.MakeGenericType(paramTypes);
                            break;
                        }

                        type = wrapperTypes.FirstOrDefault(t => t.Name == genericTypeInfo.TypeName && t.GenericTypeArguments.Length == paramTypes.Length);
                        if (type is null)
                        {
                            throw new ArgumentException(string.Format(Resources.GenericWrapperTypeNotFound.Value, genericTypeInfo.TypeName, paramTypes.Length));
                        }

                        break;
                    }

                    case TypeMemberNameSetBase.TypeInfo basicTypeInfo:
                    {
                        type = TryGetType(basicTypeInfo.TypeName);
                        if (type is null)
                        {
                            type = wrapperTypes.FirstOrDefault(t => t.Name == basicTypeInfo.TypeName);
                            if (type is null)
                            {
                                throw new ArgumentException(string.Format(Resources.WrapperTypeNotFound.Value, basicTypeInfo.TypeName));
                            }
                        }

                        if (convertToOriginalType)
                        {
                            type = GetOriginalTypeIfWrapper(type);
                        }

                        break;
                    }

                    default:
                        throw new ArgumentException();
                }

                return type;

                Type TryGetType(string name)
                {
                    Type result = Type.GetType(name);
                    if (result is null)
                    {
                        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            result = assembly.GetType(name);
                            if (!(result is null)) break;
                        }
                    }

                    return result;
                }
            }

            string GetTypeFullName(Type type)
            {
                if (type.IsArray)
                {
                    Type elementType = type.GetElementType();

                    return $"{GetTypeFullName(elementType)}[{new string(',', type.GetArrayRank() - 1)}]";
                }
                else if (type.IsConstructedGenericType)
                {
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    IEnumerable<string> typeParamNames = type.GenericTypeArguments.Select(GetTypeFullName);

                    return $"{genericTypeDefinition.FullName}[{string.Join(",", typeParamNames)}]";
                }
                else
                {
                    return type.FullName;
                }
            }

            BindingFlags CreateBindingAttribute(bool isNonPublic = false, bool isStatic = false)
            {
                BindingFlags result = BindingFlags.Default;
                result |= isNonPublic ? BindingFlags.NonPublic | BindingFlags.InvokeMethod : BindingFlags.Public;
                result |= isStatic ? BindingFlags.Static : BindingFlags.Instance;
                return result;
            }

            string GetAccessibilityDescription(bool isNonPublic = false, bool isStatic = false)
            {
                return (isNonPublic ? Resources.NonPublic : Resources.Public).Value + (isStatic ? Resources.Static : Resources.Instance).Value;
            }
            #endregion
        }
    }
}