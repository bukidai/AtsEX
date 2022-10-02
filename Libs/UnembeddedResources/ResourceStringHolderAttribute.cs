using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnembeddedResources
{
    /// <summary>
    /// 属性付きメンバーに、読み込んだ同名の <see langword="string"/> 型の <see cref="Resource{T}"/> を格納することを示します。
    /// </summary>
    public class ResourceStringHolderAttribute : Attribute
    {
        internal string LocalizerMemberName { get; }

        /// <summary>
        /// リソースの読込に使用する <see cref="ResourceLocalizer"/> を指定して、<see cref="ResourceStringHolderAttribute"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="resourceType">リソースの型。</param>
        /// <param name="localizerMemberName">リソースの読込に使用する <see cref="ResourceLocalizer"/> が格納されているメンバー (フィールドまたはプロパティ) の名前。</param>
        public ResourceStringHolderAttribute(string localizerMemberName)
        {
            LocalizerMemberName = localizerMemberName;
        }
    }
}
