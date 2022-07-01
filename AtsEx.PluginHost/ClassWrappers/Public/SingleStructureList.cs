using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 単独で設置された <see cref="Structure"/> のリストを表します。
    /// </summary>
    public sealed class SingleStructureList : MapObjectList
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<SingleStructureList>();
        }

        private SingleStructureList(IList src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="SingleStructureList"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static new SingleStructureList FromSource(object src)
        {
            if (src is null) return null;
            return new SingleStructureList((IList)src);
        }
    }
}
