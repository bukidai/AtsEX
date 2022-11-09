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
    /// 名前空間を表します。
    /// </summary>
    public sealed class Namespace
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<Namespace>("PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> NameEmpty { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> NameFormatInvalid { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();
        private static readonly string Separator = "::";

        /// <summary>
        /// ルート名前空間 (親が存在しない、トップレベルの名前空間) を取得します。
        /// </summary>
        public static readonly Namespace Root = new Namespace(null, "atsex", false);
        private static readonly Namespace UserRoot = Root.Child("user");

        static Namespace()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        /// <summary>
        /// ユーザー・プロジェクト毎に割り当てる名前空間を作成します。
        /// </summary>
        /// <remarks>
        /// 拡張機能・プラグインの開発者は、この名前空間以下にユーザー定義の名前空間を定義してください。<see cref="Root"/> 以下に直接ユーザー定義の名前空間を定義しないでください。
        /// </remarks>
        /// <param name="userName">名前空間の名前に使用するユーザー名、またはプロジェクト名。</param>
        /// <returns>作成したユーザー名前空間。</returns>
        public static Namespace GetUserNamespace(string userName) => UserRoot.Child(userName);

        private readonly Namespace Parent;

        /// <summary>
        /// この名前空間の名前を取得します。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// この名前空間の完全名を取得します。
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

        private Namespace(Namespace parent, string name, bool skipValidationCheck)
        {
            if (!skipValidationCheck) _ = CheckNameFormatValidation(name, true);

            Parent = parent;
            Name = name;

            FullName = Parent is null ? Name : Parent.FullName + Separator + Name;
        }

        /// <summary>
        /// 名前空間形式の文字列を、それと等価な <see cref="Namespace"/> に変換します。
        /// </summary>
        /// <param name="fullName">変換する文字列。</param>
        /// <returns>変換結果の名前空間。</returns>
        public static Namespace Parse(string fullName)
        {
            _ = Parse(null, fullName, out Namespace result, true);
            return result;
        }

        /// <summary>
        /// 名前空間形式の文字列を、それと等価な <see cref="Namespace"/> に変換します。戻り値は、変換が成功したかどうかを表します。
        /// </summary>
        /// <param name="fullName">変換する文字列。</param>
        /// <param name="result">変換が成功した場合、このメソッドが返されるときに、変換結果の名前空間を格納します。変換に失敗した場合は <see langword="null"/> を格納します。</param>
        /// <returns><paramref name="fullName"/> が正常に変換された場合は <see langword="true"/>。それ以外の場合は <see langword="false"/>。</returns>
        public static bool TryParse(string fullName, out Namespace result) => Parse(null, fullName, out result, true);

        private static bool Parse(Namespace parent, string fullName, out Namespace result, bool throwExceptionIfFailed)
        {
            if (fullName is null) throw new ArgumentNullException(nameof(fullName));
            if (fullName == "") throw new FormatException(Resources.Value.NameEmpty.Value);

            int separatorIndex = fullName.IndexOf(Separator);
            if (separatorIndex == -1)
            {
                result = CheckNameFormatValidation(fullName, throwExceptionIfFailed) ? new Namespace(parent, fullName, true) : null;
                return !(result is null);
            }
            else
            {
                string topMost = fullName.Substring(0, separatorIndex);
                string other = fullName.Substring(separatorIndex + Separator.Length);

                if (!CheckNameFormatValidation(topMost, throwExceptionIfFailed))
                {
                    result = null;
                    return false;
                }

                Namespace ns = new Namespace(parent, topMost, true);

                result = CheckNameFormatValidation(other, throwExceptionIfFailed) ? new Namespace(ns, other, true) : null;
                return !(result is null);
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Namespace ns && FullName == ns.FullName;

        /// <inheritdoc/>
        public override int GetHashCode() => FullName.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => FullName;

        /// <summary>
        /// この名前空間が指定された名前空間の子であるかどうかを取得します。
        /// </summary>
        /// <remarks>
        /// この名前空間と <paramref name="other"/> が等価の場合は <see langword="true"/> を返します。
        /// </remarks>
        /// <param name="other">この名前空間の親かどうかを検証する名前空間。</param>
        /// <returns>この名前空間が <paramref name="other"/> の子であるかどうか。</returns>
        public bool IsChildOf(Namespace other) => Equals(other) || (!(Parent is null) && Parent.IsChildOf(other));

        /// <summary>
        /// この名前空間の下に名前空間を作成します。
        /// </summary>
        /// <param name="name">名前空間の名前。</param>
        /// <returns>作成したサービス名前空間。</returns>
        public Namespace Child(string name) => new Namespace(this, name, true);
    }
}
