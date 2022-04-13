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
        /// <param name="atsExAssembly">AtsEX 本体 (atsex.dll) の <see cref="Assembly"/>。</param>
        /// <param name="atsExPluginHostAssembly">AtsEX プラグインホスト (atsex.pihost.dll) の <see cref="Assembly"/>。</param>
        /// <param name="loadLatestVersionIfNotSupported">実行中の BVE がサポートされる最新のバージョンを超えている場合、サポートされる最新のバージョンのプロファイルで代用するか。</param>
        /// <returns>使用するプロファイルが対応する BVE のバージョン。</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Version CreateInstance(Assembly bveAssembly, Assembly atsExAssembly, Assembly atsExPluginHostAssembly, bool loadLatestVersionIfNotSupported)
        {
            if (!(Instance is null))
            {
                throw new InvalidOperationException("インスタンスは既に生成されています。");
            }

            Version bveVersion = bveAssembly.GetName().Version;
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string defaultNamespace = $"{typeof(BveTypeCollectionProvider).Namespace}.TypeNameDefinitions";

            Version profileVersion;
            {
                string[] resourceNames = executingAssembly.GetManifestResourceNames();
                if (resourceNames.Contains($"{defaultNamespace}.{bveVersion}.xml"))
                {
                    profileVersion = bveVersion;
                }
                else
                {
                    if (loadLatestVersionIfNotSupported)
                    {
                        IEnumerable<Version> supportedAllVersions = resourceNames.Select(name => Version.Parse(name.Replace(".xml", "")));
                        IEnumerable<Version> supportedVersions = supportedAllVersions.Where(version => version.Major == bveVersion.Major);
                        if (supportedVersions.Any())
                        {
                            Version latestVersion = supportedVersions.Max();
                            Version oldestVersion = supportedVersions.Min();
                            if (bveVersion < latestVersion)
                            {
                                if (bveVersion < oldestVersion)
                                {
                                    throw new NotSupportedException($"BVE バージョン {bveVersion} には対応していません。{oldestVersion} 以降のみサポートしています。");
                                }
                                else
                                {
                                    throw new NotSupportedException($"BVE バージョン {bveVersion} は認識されていません。サポートされないバージョンであるか、不正なバージョンです。");
                                }
                            }

                            profileVersion = latestVersion;
                        }
                        else
                        {
                            IEnumerable<Version> newerVersions = supportedAllVersions.Where(version => version.Major < bveVersion.Major);
                            if (newerVersions.Any())
                            {
                                profileVersion = newerVersions.Max();
                            }
                            else
                            {
                                throw new NotSupportedException($"BVE バージョン {bveVersion} には対応していません。{supportedAllVersions.Min().Major}.x 以降のみサポートしています。");
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"BVE バージョン {bveVersion} には対応していません。");
                    }
                }
            }

            IEnumerable<TypeMemberNameCollection> nameCollection;
            using (Stream doc = executingAssembly.GetManifestResourceStream($"{defaultNamespace}.{profileVersion}.xml"))
            {
                using (Stream schema = executingAssembly.GetManifestResourceStream($"{defaultNamespace}.BveTypesXmlSchema.xsd"))
                {
                    nameCollection = BveTypeNameDefinitionLoader.LoadFile(doc, schema).ToArray();
                }
            }

            IEnumerable<Type> wrapperTypes = atsExPluginHostAssembly.GetTypes().Concat(atsExAssembly.GetTypes()).Where(type => type.IsClass && type.IsSubclassOf(typeof(ClassWrapper)));
            IEnumerable<Type> originalTypes = bveAssembly.GetTypes();

            IEnumerable<KeyValuePair<Type, Type>> wrapperOriginalTypePairs = nameCollection.Select(src =>
            {
                Type wrapperType = ParseWrapperTypeName(src.WrapperTypeName);
                if (wrapperType is null)
                {
                    throw new KeyNotFoundException($"ラッパー型 '{src.WrapperTypeName}' は存在しません。");
                }

                Type originalType = ParseOriginalTypeName(src.OriginalTypeName);
                if (originalType is null)
                {
                    throw new KeyNotFoundException($"ラッパー型 '{src.WrapperTypeName}' のオリジナル型 '{src.OriginalTypeName}' は BVE 内に存在しません。");
                }

                return new KeyValuePair<Type, Type>(wrapperType, originalType);
            });

            IEnumerable<BveTypeMemberCollection> types = nameCollection.Select(src =>
            {
                Type wrapperType = ParseWrapperTypeName(src.WrapperTypeName);
                Type originalType = wrapperOriginalTypePairs.First(x => x.Key == wrapperType).Value;

                SortedList<Type[], ConstructorInfo> constructors = new SortedList<Type[], ConstructorInfo>(originalType.GetConstructors(SearchAllBindingAttribute).ToDictionary(
                    c => c.GetParameters().Select(p => GetWrapperTypeIfOriginal(p.ParameterType)).ToArray(),
                    c => c),
                    new TypeArrayComparer());

                SortedList<string, MethodInfo> propertyGetters = new SortedList<string, MethodInfo>(src.PropertyGetters.Select(getterInfo =>
                {
                    PropertyInfo wrapperProperty = wrapperType.GetProperty(getterInfo.WrapperName, CreateBindingAttribute(getterInfo.IsWrapperPrivate, getterInfo.IsWrapperStatic));
                    if (wrapperProperty is null)
                    {
                        throw new KeyNotFoundException($"{GetAccessibilityDescription(getterInfo.IsWrapperPrivate)}プロパティ '{getterInfo.WrapperName}' は" +
                            $"ラッパー型 '{src.WrapperTypeName}' に存在しません。");
                    }

                    MethodInfo originalGetter = originalType.GetMethod(getterInfo.OriginalName, CreateBindingAttribute(getterInfo.IsOriginalPrivate, getterInfo.IsOriginalStatic), null, Type.EmptyTypes, null);
                    if (originalGetter is null)
                    {
                        throw new KeyNotFoundException($"{GetAccessibilityDescription(getterInfo.IsOriginalPrivate, getterInfo.IsOriginalStatic)}プロパティ get アクセサ '{getterInfo.OriginalName}' は" +
                            $"ラッパー型 '{src.WrapperTypeName}' のオリジナル型 '{src.OriginalTypeName}' に存在しません。");
                    }

                    return new KeyValuePair<string, MethodInfo>(wrapperProperty.Name, originalGetter);
                }).ToDictionary(x => x.Key, x => x.Value));

                SortedList<string, MethodInfo> propertySetters = new SortedList<string, MethodInfo>(src.PropertySetters.Select(setterInfo =>
                {
                    PropertyInfo wrapperProperty = wrapperType.GetProperty(setterInfo.WrapperName, CreateBindingAttribute(setterInfo.IsWrapperPrivate, setterInfo.IsWrapperStatic));
                    if (wrapperProperty is null)
                    {
                        throw new KeyNotFoundException($"{GetAccessibilityDescription(setterInfo.IsWrapperPrivate)}プロパティ '{setterInfo.WrapperName}' は" +
                            $"ラッパー型 '{src.WrapperTypeName}' に存在しません。");
                    }

                    Type originalParamType = GetOriginalTypeIfWrapper(wrapperProperty.PropertyType,
                        $"ラッパープロパティ '{src.WrapperTypeName}.{setterInfo.WrapperName}' の型 '{wrapperProperty.PropertyType.Name}' のオリジナル型が見つかりませんでした。");
                    MethodInfo originalSetter = originalType.GetMethod(setterInfo.OriginalName, CreateBindingAttribute(setterInfo.IsOriginalPrivate, setterInfo.IsOriginalStatic), null, new Type[] { originalParamType }, null);
                    if (originalSetter is null)
                    {
                        throw new KeyNotFoundException($"{GetAccessibilityDescription(setterInfo.IsOriginalPrivate, setterInfo.IsOriginalStatic)}プロパティ set アクセサ '{setterInfo.OriginalName}' は" +
                            $"ラッパー型 '{src.WrapperTypeName}' のオリジナル型 '{src.OriginalTypeName}' に存在しません。");
                    }

                    return new KeyValuePair<string, MethodInfo>(wrapperProperty.Name, originalSetter);
                }).ToDictionary(x => x.Key, x => x.Value));

                SortedList<string, FieldInfo> fields = new SortedList<string, FieldInfo>(src.Fields.Select(fieldInfo =>
                {
                    PropertyInfo wrapperProperty = wrapperType.GetProperty(fieldInfo.WrapperName, CreateBindingAttribute(fieldInfo.IsWrapperPrivate, fieldInfo.IsWrapperStatic));
                    if (wrapperProperty is null)
                    {
                        throw new KeyNotFoundException($"{GetAccessibilityDescription(fieldInfo.IsWrapperPrivate)}プロパティ '{fieldInfo.WrapperName}' は" +
                            $"ラッパー型 '{src.WrapperTypeName}' に存在しません。");
                    }

                    FieldInfo originalField = originalType.GetField(fieldInfo.OriginalName, CreateBindingAttribute(fieldInfo.IsOriginalPrivate, fieldInfo.IsOriginalStatic));
                    if (originalField is null)
                    {
                        throw new KeyNotFoundException($"{GetAccessibilityDescription(fieldInfo.IsOriginalPrivate, fieldInfo.IsOriginalStatic)}フィールド '{fieldInfo.OriginalName}' は" +
                            $"ラッパー型 '{src.WrapperTypeName}' のオリジナル型 '{src.OriginalTypeName}' に存在しません。");
                    }

                    return new KeyValuePair<string, FieldInfo>(wrapperProperty.Name, originalField);
                }).ToDictionary(x => x.Key, x => x.Value));

                SortedList<(string, Type[]), MethodInfo> methods = new SortedList<(string, Type[]), MethodInfo>(src.Methods.Select(methodInfo =>
                {
                    Type[] wrapperMethodParams = methodInfo.WrapperParamNames.Select(p => ParseTypeName(p, false)).ToArray();
                    MethodInfo wrapperMethod = wrapperType.GetMethod(methodInfo.WrapperName, CreateBindingAttribute(methodInfo.IsWrapperPrivate, methodInfo.IsWrapperStatic), null, wrapperMethodParams, null);
                    if (wrapperMethod is null)
                    {
                        throw new KeyNotFoundException($"パラメータ '{string.Join(", ", wrapperMethodParams.Select(GetTypeFullName))}' をもつ" +
                            $"{GetAccessibilityDescription(methodInfo.IsWrapperPrivate)}メソッド '{methodInfo.WrapperName}' は" +
                            $"ラッパー型 '{src.WrapperTypeName}' に存在しません。");
                    }

                    Type[] originalMethodParams = methodInfo.WrapperParamNames.Select(p => ParseTypeName(p, true)).ToArray();
                    MethodInfo originalMethod = originalType.GetMethod(methodInfo.OriginalName, CreateBindingAttribute(methodInfo.IsOriginalPrivate, methodInfo.IsOriginalStatic), null, originalMethodParams, null);
                    if (originalMethod is null)
                    {
                        throw new KeyNotFoundException($"パラメータ '{string.Join(", ", originalMethodParams.Select(GetTypeFullName))}' をもつ" +
                            $"{GetAccessibilityDescription(methodInfo.IsOriginalPrivate, methodInfo.IsOriginalStatic)}メソッド '{methodInfo.OriginalName}' は" +
                            $"ラッパー型 '{src.WrapperTypeName}' のオリジナル型 '{src.OriginalTypeName}' に存在しません。");
                    }

                    return new KeyValuePair<(string, Type[]), MethodInfo>((wrapperMethod.Name, wrapperMethodParams), originalMethod);
                }).ToDictionary(x => x.Key, x => x.Value), new StringTypeArrayTupleComparer());


                BveTypeMemberCollection members = new BveTypeMemberCollection(wrapperType, originalType, constructors, propertyGetters, propertySetters, fields, methods);
                return members;
            });

            Instance = new BveTypeCollectionProvider(types, typeof(ClassWrapper));

            return profileVersion;


            #region メソッド
            Type ParseWrapperTypeName(string typeName) => wrapperTypes.FirstOrDefault(wrapperType => wrapperType.Name == typeName);
            Type ParseOriginalTypeName(string typeName) => originalTypes.FirstOrDefault(originalType => originalType.FullName == typeName);

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
                    Type originalType = wrapperOriginalTypePairs.FirstOrDefault(x => x.Key == type).Value;
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

                if (type.IsClass && type.IsSubclassOf(typeof(ClassWrapper)))
                {
                    Type wrapperType = wrapperOriginalTypePairs.FirstOrDefault(x => x.Value == type).Key;
                    if (wrapperType is null)
                    {
                        if (typeLoadExceptionMessage is null)
                        {
                            typeLoadExceptionMessage = $"型 '{type.Name}' のラッパー型が見つかりませんでした。";
                        }

                        throw new TypeLoadException(typeLoadExceptionMessage);
                    }

                    return wrapperType;
                }
                else
                {
                    return type;
                }
            }

            Type ParseTypeName(TypeMemberNameCollection.TypeInfoBase typeInfo, bool convertToOriginalType)
            {
                Type type;
                switch (typeInfo)
                {
                    case TypeMemberNameCollection.ArrayTypeInfo arrayTypeInfo:
                        {
                            Type baseType = ParseTypeName(arrayTypeInfo.BaseType, convertToOriginalType);
                            type = baseType.MakeArrayType(arrayTypeInfo.DimensionCount);
                        }
                        break;

                    case TypeMemberNameCollection.GenericTypeInfo genericTypeInfo:
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

                    case TypeMemberNameCollection.TypeInfo basicTypeInfo:
                        {
                            type = Type.GetType(basicTypeInfo.TypeName);
                            if (type is null)
                            {
                                type = ParseWrapperTypeName(basicTypeInfo.TypeName);
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