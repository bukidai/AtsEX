using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 自列車の通過を検知可能な <see cref="MapObjectBase"/> のリストを表します。
    /// </summary>
    public class MapFunctionList : MapObjectList
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<MapFunctionList>();
        }

        private MapFunctionList(IList src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="MapFunctionList"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static new MapFunctionList FromSource(object src) => src is null ? null : new MapFunctionList((IList)src);
    }
}
