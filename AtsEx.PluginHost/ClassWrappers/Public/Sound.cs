using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// サウンドを表します。
    /// </summary>
    public sealed class Sound : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<Sound>();
        }

        private Sound(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Sound"/> クラスのインスタンス。</returns>
        public static Sound FromSource(object src)
        {
            if (src is null) return null;
            return new Sound(src);
        }
    }
}
