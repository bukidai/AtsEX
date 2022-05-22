using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    public partial class BveTypeCollectionProvider : IDisposable
    {
        public static BveTypeCollectionProvider Instance { get; protected set; } = null;

        protected const BindingFlags SearchAllBindingAttribute = BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

        /// <summary>
        /// <see cref="BveTypeCollectionProvider"/> のインスタンスを作成します。
        /// </summary>
        /// <param name="bveAssembly">BVE の <see cref="Assembly"/>。</param>
        /// <param name="atsExAssembly">AtsEX 本体 (AtsEx.dll) の <see cref="Assembly"/>。</param>
        /// <param name="atsExPluginHostAssembly">AtsEX プラグインホスト (AtsEx.PluginHost.dll) の <see cref="Assembly"/>。</param>
        /// <param name="allowNotSupportedVersion">実行中の BVE がサポートされないバージョンの場合、他のバージョン向けのプロファイルで代用するか。</param>
        /// <returns>使用するプロファイルが対応する BVE のバージョン。</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Version CreateInstance(Assembly bveAssembly, Assembly atsExAssembly, Assembly atsExPluginHostAssembly, bool allowNotSupportedVersion)
        {
            if (!(Instance is null))
            {
                throw new InvalidOperationException("インスタンスは既に生成されています。");
            }

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            ProfileSelector profileSelector = new ProfileSelector(bveAssembly);
            Version profileVersion;
            List<TypeMemberNameCollectionBase> nameCollection;
            using (ProfileInfo profile = profileSelector.GetProfileStream(allowNotSupportedVersion))
            {
                profileVersion = profile.Version;

                using (Stream schema = profileSelector.GetSchemaStream())
                {
                    nameCollection = BveTypeNameDefinitionLoader.LoadFile(profile.Stream, schema);
                }
            }

            IEnumerable<Type> wrapperTypes = atsExPluginHostAssembly.GetTypes().Concat(atsExAssembly.GetTypes()).Where(type => (type.IsClass && type.IsSubclassOf(typeof(ClassWrapper))) || type.IsEnum);
            IEnumerable<Type> originalTypes = bveAssembly.GetTypes();

            TypeInfoGenerator typeInfoGenerator = new TypeInfoGenerator(bveAssembly, atsExAssembly);
            IEnumerable<TypeInfo> typeInfos = typeInfoGenerator.ConvertTypeMemberNameCollections(nameCollection);

            IEnumerable<TypeMemberCollectionBase> types = nameCollection.Select<TypeMemberNameCollectionBase, TypeMemberCollectionBase>(src =>
            {
                TypeInfo typeInfo = typeInfos.First(t => t.Src == src);

                Type wrapperType = typeInfo.WrapperType;
                Type originalType = typeInfo.OriginalType;

                switch (src)
                {
                    case EnumMemberNameCollection enumSrc:
                        {
                            EnumMemberCollection members = new EnumMemberCollection(wrapperType, originalType);
                            return members;
                        }

                    case ClassMemberNameCollection classSrc:
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
                                    throw new KeyNotFoundException($"{GetAccessibilityDescription(getterInfo.IsWrapperPrivate)}プロパティ '{getterInfo.WrapperName}' は" +
                                        $"ラッパー型 '{classSrc.WrapperTypeName}' に存在しません。");
                                }

                                MethodInfo originalGetter = originalType.GetMethod(getterInfo.OriginalName, CreateBindingAttribute(getterInfo.IsOriginalPrivate, getterInfo.IsOriginalStatic), null, Type.EmptyTypes, null);
                                if (originalGetter is null)
                                {
                                    throw new KeyNotFoundException($"{GetAccessibilityDescription(getterInfo.IsOriginalPrivate, getterInfo.IsOriginalStatic)}プロパティ get アクセサ '{getterInfo.OriginalName}' は" +
                                        $"ラッパー型 '{classSrc.WrapperTypeName}' のオリジナル型 '{classSrc.OriginalTypeName}' に存在しません。");
                                }

                                return new KeyValuePair<string, MethodInfo>(wrapperProperty.Name, originalGetter);
                            }).ToDictionary(x => x.Key, x => x.Value));

                            SortedList<string, MethodInfo> propertySetters = new SortedList<string, MethodInfo>(classSrc.PropertySetters.Select(setterInfo =>
                            {
                                PropertyInfo wrapperProperty = wrapperType.GetProperty(setterInfo.WrapperName, CreateBindingAttribute(setterInfo.IsWrapperPrivate, setterInfo.IsWrapperStatic));
                                if (wrapperProperty is null)
                                {
                                    throw new KeyNotFoundException($"{GetAccessibilityDescription(setterInfo.IsWrapperPrivate)}プロパティ '{setterInfo.WrapperName}' は" +
                                        $"ラッパー型 '{classSrc.WrapperTypeName}' に存在しません。");
                                }

                                Type originalParamType = GetOriginalTypeIfWrapper(wrapperProperty.PropertyType,
                                    $"ラッパープロパティ '{classSrc.WrapperTypeName}.{setterInfo.WrapperName}' の型 '{wrapperProperty.PropertyType.Name}' のオリジナル型が見つかりませんでした。");
                                MethodInfo originalSetter = originalType.GetMethod(setterInfo.OriginalName, CreateBindingAttribute(setterInfo.IsOriginalPrivate, setterInfo.IsOriginalStatic), null, new Type[] { originalParamType }, null);
                                if (originalSetter is null)
                                {
                                    throw new KeyNotFoundException($"{GetAccessibilityDescription(setterInfo.IsOriginalPrivate, setterInfo.IsOriginalStatic)}プロパティ set アクセサ '{setterInfo.OriginalName}' は" +
                                        $"ラッパー型 '{classSrc.WrapperTypeName}' のオリジナル型 '{classSrc.OriginalTypeName}' に存在しません。");
                                }

                                return new KeyValuePair<string, MethodInfo>(wrapperProperty.Name, originalSetter);
                            }).ToDictionary(x => x.Key, x => x.Value));

                            SortedList<string, FieldInfo> fields = new SortedList<string, FieldInfo>(classSrc.Fields.Select(fieldInfo =>
                            {
                                PropertyInfo wrapperProperty = wrapperType.GetProperty(fieldInfo.WrapperName, CreateBindingAttribute(fieldInfo.IsWrapperPrivate, fieldInfo.IsWrapperStatic));
                                if (wrapperProperty is null)
                                {
                                    throw new KeyNotFoundException($"{GetAccessibilityDescription(fieldInfo.IsWrapperPrivate)}プロパティ '{fieldInfo.WrapperName}' は" +
                                        $"ラッパー型 '{classSrc.WrapperTypeName}' に存在しません。");
                                }

                                FieldInfo originalField = originalType.GetField(fieldInfo.OriginalName, CreateBindingAttribute(fieldInfo.IsOriginalPrivate, fieldInfo.IsOriginalStatic));
                                if (originalField is null)
                                {
                                    throw new KeyNotFoundException($"{GetAccessibilityDescription(fieldInfo.IsOriginalPrivate, fieldInfo.IsOriginalStatic)}フィールド '{fieldInfo.OriginalName}' は" +
                                        $"ラッパー型 '{classSrc.WrapperTypeName}' のオリジナル型 '{classSrc.OriginalTypeName}' に存在しません。");
                                }

                                return new KeyValuePair<string, FieldInfo>(wrapperProperty.Name, originalField);
                            }).ToDictionary(x => x.Key, x => x.Value));

                            SortedList<(string, Type[]), MethodInfo> methods = new SortedList<(string, Type[]), MethodInfo>(classSrc.Methods.Select(methodInfo =>
                            {
                                Type[] wrapperMethodParams = methodInfo.WrapperParamNames.Select(p => ParseTypeName(p, false)).ToArray();
                                MethodInfo wrapperMethod = wrapperType.GetMethod(methodInfo.WrapperName, CreateBindingAttribute(methodInfo.IsWrapperPrivate, methodInfo.IsWrapperStatic), null, wrapperMethodParams, null);
                                if (wrapperMethod is null)
                                {
                                    throw new KeyNotFoundException($"パラメータ '{string.Join(", ", wrapperMethodParams.Select(GetTypeFullName))}' をもつ" +
                                        $"{GetAccessibilityDescription(methodInfo.IsWrapperPrivate)}メソッド '{methodInfo.WrapperName}' は" +
                                        $"ラッパー型 '{classSrc.WrapperTypeName}' に存在しません。");
                                }

                                Type[] originalMethodParams = methodInfo.WrapperParamNames.Select(p => ParseTypeName(p, true)).ToArray();
                                MethodInfo originalMethod = originalType.GetMethod(methodInfo.OriginalName, CreateBindingAttribute(methodInfo.IsOriginalPrivate, methodInfo.IsOriginalStatic), null, originalMethodParams, null);
                                if (originalMethod is null)
                                {
                                    throw new KeyNotFoundException($"パラメータ '{string.Join(", ", originalMethodParams.Select(GetTypeFullName))}' をもつ" +
                                        $"{GetAccessibilityDescription(methodInfo.IsOriginalPrivate, methodInfo.IsOriginalStatic)}メソッド '{methodInfo.OriginalName}' は" +
                                        $"ラッパー型 '{classSrc.WrapperTypeName}' のオリジナル型 '{classSrc.OriginalTypeName}' に存在しません。");
                                }

                                return new KeyValuePair<(string, Type[]), MethodInfo>((wrapperMethod.Name, wrapperMethodParams), originalMethod);
                            }).ToDictionary(x => x.Key, x => x.Value), new StringTypeArrayTupleComparer());


                            ClassMemberCollection members = new ClassMemberCollection(wrapperType, originalType, constructors, propertyGetters, propertySetters, fields, methods);
                            return members;
                        }

                    default:
                        throw new NotImplementedException($"{nameof(TypeMemberNameCollectionBase)} の派生クラス {src.GetType().Name} は認識されていません。");

                }
            });

            Instance = new BveTypeCollectionProvider(types, typeof(ClassWrapper));

            return profileVersion;


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

                    if (genericTypeDefinition == typeof(WrappedSortedList<,>))
                    {
                        genericTypeDefinition = typeof(SortedList<,>);
                    }

                    type = genericTypeDefinition.MakeGenericType(typeParams);
                }

                if (type.IsClass && type.IsSubclassOf(typeof(ClassWrapper)))
                {
                    Type originalType = typeInfos.FirstOrDefault(x => x.WrapperType == type)?.OriginalType;
                    if (originalType is null)
                    {
                        if (typeLoadExceptionMessage is null)
                        {
                            typeLoadExceptionMessage = $"型 '{type.Name}' のオリジナル型が見つかりませんでした。";
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

                    if (genericTypeDefinition == typeof(SortedList<,>))
                    {
                        genericTypeDefinition = typeof(WrappedSortedList<,>);
                    }

                    type = genericTypeDefinition.MakeGenericType(typeParams);
                }

                Type wrapperType = typeInfos.FirstOrDefault(x => x.OriginalType == type)?.WrapperType;
                return wrapperType ?? type;
            }

            Type ParseTypeName(TypeMemberNameCollectionBase.TypeInfoBase typeInfo, bool convertToOriginalType)
            {
                Type type;
                switch (typeInfo)
                {
                    case TypeMemberNameCollectionBase.ArrayTypeInfo arrayTypeInfo:
                        {
                            Type baseType = ParseTypeName(arrayTypeInfo.BaseType, convertToOriginalType);
                            type = baseType.MakeArrayType(arrayTypeInfo.DimensionCount);
                        }
                        break;

                    case TypeMemberNameCollectionBase.GenericTypeInfo genericTypeInfo:
                        {
                            Type[] paramTypes = genericTypeInfo.TypeParams.Select(t => ParseTypeName(t, convertToOriginalType)).ToArray();

                            Type genericDefinitionType = Type.GetType($"{genericTypeInfo.TypeName}`{paramTypes.Length}");
                            if (!(genericDefinitionType is null))
                            {
                                type = genericDefinitionType.MakeGenericType(paramTypes);
                                break;
                            }

                            type = wrapperTypes.FirstOrDefault(t => t.Name == genericTypeInfo.TypeName && t.GenericTypeArguments.Length == paramTypes.Length);
                            if (type is null)
                            {
                                throw new ArgumentException($"型名が '{genericTypeInfo.TypeName}'、型パラメータが {paramTypes.Length} 個のジェネリック型は存在しません。");
                            }
                        }
                        break;

                    case TypeMemberNameCollectionBase.TypeInfo basicTypeInfo:
                        {
                            type = Type.GetType(basicTypeInfo.TypeName);
                            if (type is null)
                            {
                                type = wrapperTypes.FirstOrDefault(t => t.Name == basicTypeInfo.TypeName);
                                if (type is null)
                                {
                                    throw new ArgumentException($"型 '{basicTypeInfo.TypeName}' は存在しません。");
                                }
                            }

                            if (convertToOriginalType)
                            {
                                type = GetOriginalTypeIfWrapper(type);
                            }
                        }
                        break;

                    default:
                        throw new ArgumentException();
                }

                return type;
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
                return $"パブリック{(isNonPublic ? "でない" : "")}{(isStatic ? "静的" : "インスタンス")}";
            }
            #endregion
        }
    }
}