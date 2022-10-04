using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using UnembeddedResources;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    internal class TypeParser
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<TypeParser>("PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> IllegalCharacterDetectedInAlias { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> IllegalTypeDetected { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> TypeNotFound { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly ResourceSet Resources = new ResourceSet();
        private const char Separator = ';';

        private readonly Dictionary<string, Type> Cache = new Dictionary<string, Type>();
        private readonly Dictionary<string, Type> SpecializedTypes;

        private readonly string SpecializedTypeAlias;

        /// <summary>
        /// <see cref="TypeParser"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="specializedTypeAlias">名前空間の指定をせずに型を参照する場合に使用するエイリアス。</param>
        /// <param name="specializeTypes">名前空間の指定をせずに参照可能な型の一覧。</param>
        public TypeParser(string specializedTypeAlias, IEnumerable<Type> specializeTypes)
        {
#if DEBUG
            if (!Regex.IsMatch(specializedTypeAlias, @"^[0-9a-zA-Z]+$"))
            {
                throw new ArgumentException(Resources.IllegalCharacterDetectedInAlias.Value, specializedTypeAlias);
            }
            else if (specializeTypes.Any(t => t.IsArray || t.IsConstructedGenericType))
            {
                throw new ArgumentException(Resources.IllegalTypeDetected.Value, nameof(specializeTypes));
            }
#endif

            SpecializedTypeAlias = specializedTypeAlias + ":";
            SpecializedTypes = new Dictionary<string, Type>(specializeTypes.ToDictionary(t => t.FullName.Split('.').Last(), t => t));
        }

        public Type Parse(string text, Converter<Type, Type> specializedTypeConverter = null)
        {
            text = text.Trim();

            if (Cache.TryGetValue(text, out Type cachedType)) return cachedType;

            int specializedTypeAliasIndex = text.IndexOf(SpecializedTypeAlias);
            if (specializedTypeAliasIndex != -1)
            {
                int beginIndex = specializedTypeAliasIndex + SpecializedTypeAlias.Length;

                int openBracketIndex = text.IndexOf('[', beginIndex);
                int closeBracketIndex = text.IndexOf(']', beginIndex);
                int separatorIndex = text.IndexOf(Separator, beginIndex);
                int endIndex = GetMinIndex(text.Length, openBracketIndex, closeBracketIndex, separatorIndex);

                string specializedTypeName = text.Substring(beginIndex, endIndex - beginIndex);
                Type parsedType = ParseSingleSpecializedTypeName(specializedTypeName);
                Type convertedType = specializedTypeConverter is null ? parsedType : specializedTypeConverter(parsedType);

                string replaceTo = specializedTypeAliasIndex > 0 && text[specializedTypeAliasIndex - 1] == '[' && endIndex == closeBracketIndex
                    ? $"[{convertedType.AssemblyQualifiedName}]" : convertedType.AssemblyQualifiedName;

                string replacedText = text.Replace(SpecializedTypeAlias + specializedTypeName, replaceTo);
                return Parse(replacedText, specializedTypeConverter);
            }

            Type result = Type.GetType(text);

            if (result is null)
            {
                throw new ArgumentException(Resources.TypeNotFound.Value, nameof(text));
            }

            Cache.Add(text, result);
            return result;


            int GetMinIndex(int defaultIndex, params int[] indices)
            {
                int? min = null;
                foreach (int index in indices)
                {
                    if (index == -1) continue;
                    if (min is null || index < min) min = index;
                }

                return min ?? defaultIndex;
            }
        }

        public Type ParseSingleSpecializedTypeName(string typeName)
        {
            Type type = SpecializedTypes[typeName];
            return type;
        }

        public IEnumerable<Type> ParseArray(string text, Converter<Type, Type> specializedTypeConverter = null)
        {
            switch (text)
            {
                case null:
                case "":
                case "void":
                case "System.Void":
                    return Enumerable.Empty<Type>();

                default:
                    string[] textArray = text.Split(Separator);
                    return textArray.Select(x => Parse(x, specializedTypeConverter));
            }
        }
    }
}
