using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

namespace AtsEx.PluginHost.MapStatements
{
    /// <summary>
    /// 識別子を、それに対応する <typeparamref name="TEnum"/> 型の値に変換するための機能を提供します。
    /// </summary>
    /// <typeparam name="TEnum">変換先となる列挙型。</typeparam>
    public class IdentifierToEnumConverter<TEnum> where TEnum : struct
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType(typeof(IdentifierToEnumConverter<>), "PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> TypeNotEnum { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static IdentifierToEnumConverter()
        {
            if (!typeof(TEnum).IsEnum) throw new InvalidOperationException(string.Format(Resources.Value.TypeNotEnum.Value, typeof(TEnum).FullName));

#if DEBUG
            _ = Resources.Value;
#endif
        }

        /// <summary>
        /// 識別子と、それに対応する <typeparamref name="TEnum"/> 型の値のペアを格納します。
        /// </summary>
        protected readonly Dictionary<Identifier, TEnum> Dictionary;

        /// <summary>
        /// <see cref="IdentifierToEnumConverter{TEnum}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="parent">変換対象の <see cref="Identifier"/> の親となる識別子。</param>
        public IdentifierToEnumConverter(Identifier parent)
        {
            Dictionary<Identifier, TEnum> dictionary = new Dictionary<Identifier, TEnum>();
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                Identifier identifier = new Identifier(parent, enumValue.ToString().ToLowerInvariant());
                dictionary.Add(identifier, enumValue);
            }

            Dictionary = dictionary;
        }

        /// <summary>
        /// 指定した識別子を、それに対応する <typeparamref name="TEnum"/> 型の値に変換します。
        /// </summary>
        /// <param name="value">変換する識別子。</param>
        /// <param name="result">変換に成功した場合は変換結果の値、失敗した場合は <typeparamref name="TEnum"/> 型の既定の値。</param>
        /// <returns>変換に成功した場合は <see langword="true"/>、失敗した場合は <see langword="false"/>。</returns>
        public virtual bool TryConvert(Identifier value, out TEnum result) => Dictionary.TryGetValue(value, out result);
    }
}
