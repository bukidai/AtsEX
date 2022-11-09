using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using UnembeddedResources;

namespace AtsEx.PluginHost.MapStatements
{
    /// <summary>
    /// <see cref="IHeader"/> や <see cref="IStatement"/> を区別するための識別子 (名前) を表します。
    /// </summary>
    public sealed class Identifier
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<Identifier>("PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> NameEmpty { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> NameFormatInvalid { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        private static readonly string NamespaceSeparator = "::";
        private static readonly string IdentifierSeparator = ".";

        static Identifier()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        /// <summary>
        /// この識別子の名前空間を取得します。
        /// </summary>
        public Namespace Namespace { get; }

        /// <summary>
        /// この識別子の親となる <see cref="Identifier"/> を取得します。名前空間直下に定義されており、親が存在しない場合は <see langword="null"/> となります。
        /// </summary>
        public Identifier Parent { get; }

        /// <summary>
        /// この識別子の名前を取得します。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// この識別子の完全修飾名を取得します。
        /// </summary>
        public string FullName { get; }

        private static bool CheckNameFormatValidation(string name, bool throwExceptionIfInvalid)
        {
#pragma warning disable IDE0046 // 条件式に変換します
            if (name is null) return throwExceptionIfInvalid ? throw new ArgumentNullException(nameof(name)) : false;
            if (name == "") return throwExceptionIfInvalid ? throw new FormatException(Resources.Value.NameEmpty.Value) : false;
            if (!Regex.IsMatch(name, "[a-z0-9_]+")) return throwExceptionIfInvalid ? throw new FormatException(Resources.Value.NameFormatInvalid.Value) : false;
#pragma warning restore IDE0046 // 条件式に変換します

            return true;
        }

        private Identifier(Namespace ns, Identifier parent, string name, bool skipValidationCheck)
        {
            if (!skipValidationCheck) _ = CheckNameFormatValidation(name, true);

            Parent = parent;
            Name = name;
            Namespace = ns;
        }

        private Identifier(Namespace ns, string name, bool skipValidationCheck) : this(ns, null, name, skipValidationCheck)
        {
            FullName = (Namespace is null ? "" : Namespace.FullName + NamespaceSeparator) + Name;
        }

        /// <summary>
        /// 親の識別子をもたず、名前空間直下に定義される <see cref="Identifier"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="ns">識別子の名前空間。</param>
        /// <param name="name">識別子の名前。</param>
        public Identifier(Namespace ns, string name) : this(ns, name, false)
        {
        }

        private Identifier(Identifier parent, string name, bool skipValidationCheck) : this(parent.Namespace, parent, name, skipValidationCheck)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));

            FullName = parent.FullName + IdentifierSeparator + Name;
        }

        /// <summary>
        /// 親の識別子をもつ <see cref="Identifier"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="parent">親の識別子。</param>
        /// <param name="name">識別子の名前。</param>
        public Identifier(Identifier parent, string name) : this(parent, name, false)
        {
        }

        /// <summary>
        /// 識別子の完全修飾名形式の文字列を、それと等価な <see cref="Identifier"/> に変換します。
        /// </summary>
        /// <param name="fullName">変換する文字列。</param>
        /// <returns>変換結果の識別子。</returns>
        public static Identifier Parse(string fullName)
        {
            _ = Parse(fullName, out Identifier result, true);
            return result;
        }

        /// <summary>
        /// 識別子の完全修飾名形式の文字列を、それと等価な <see cref="Identifier"/> に変換します。戻り値は、変換が成功したかどうかを表します。
        /// </summary>
        /// <param name="fullName">変換する文字列。</param>
        /// <param name="result">変換が成功した場合、このメソッドが返されるときに、変換結果の識別子を格納します。変換に失敗した場合は <see langword="null"/> を格納します。</param>
        /// <returns><paramref name="fullName"/> が正常に変換された場合は <see langword="true"/>。それ以外の場合は <see langword="false"/>。</returns>
        public static bool TryParse(string fullName, out Identifier result) => Parse(fullName, out result, false);

        private static bool Parse(string fullName, out Identifier result, bool throwExceptionIfFailed)
        {
            int lastIdentifierSeparatorIndex = fullName.LastIndexOf(IdentifierSeparator);
            if (lastIdentifierSeparatorIndex == -1)
            {
                int namespaceSeparatorIndex = fullName.LastIndexOf(NamespaceSeparator);
                if (namespaceSeparatorIndex == -1)
                {
                    result = CheckNameFormatValidation(fullName, throwExceptionIfFailed) ? new Identifier((Namespace)null, fullName, true) : null;
                    return !(result is null);
                }
                else
                {
                    string headerNamespace = fullName.Substring(0, namespaceSeparatorIndex);
                    string headerName = fullName.Substring(namespaceSeparatorIndex + NamespaceSeparator.Length);

                    Namespace ns;
                    if (throwExceptionIfFailed)
                    {
                        ns = Namespace.Parse(headerNamespace);
                    }
                    else
                    {
                        if (!Namespace.TryParse(headerNamespace, out ns))
                        {
                            result = null;
                            return false;
                        }
                    }

                    result = CheckNameFormatValidation(headerName, throwExceptionIfFailed) ? new Identifier(ns, headerName, true) : null;
                    return !(result is null);
                }
            }
            else
            {
                string parentFullName = fullName.Substring(0, lastIdentifierSeparatorIndex);
                string name = fullName.Substring(lastIdentifierSeparatorIndex + IdentifierSeparator.Length);

                if (!Parse(parentFullName, out Identifier parent, throwExceptionIfFailed))
                {
                    result = null;
                    return false;
                }

                result = CheckNameFormatValidation(name, throwExceptionIfFailed) ? new Identifier(parent, name, true) : null;
                return !(result is null);
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Identifier identifier && FullName == identifier.FullName;

        /// <inheritdoc/>
        public override int GetHashCode() => FullName.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => FullName;
    }
}
