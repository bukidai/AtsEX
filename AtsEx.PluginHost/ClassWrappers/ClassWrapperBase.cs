using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// すべてのクラスラッパーの基本クラスを表します。
    /// </summary>
    public abstract class ClassWrapperBase
    {
        /// <summary>
        /// ラップされているオリジナル オブジェクトを取得します。
        /// </summary>
        public dynamic Src { get; }

        /// <summary>
        /// <see cref="ClassWrapperBase"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> が <see langword="null"/> です。</exception>
        public ClassWrapperBase(object src)
        {
            if (src is null) throw new ArgumentNullException();

            Src = src;
        }

        public override bool Equals(object obj)
        {
            return obj is ClassWrapperBase classWrapper && (bool)(Src as object).Equals(classWrapper.Src);
        }

        public override int GetHashCode()
        {
            return (Src as object).GetHashCode();
        }

        public override string ToString()
        {
            return (Src as object).ToString();
        }
    }
}