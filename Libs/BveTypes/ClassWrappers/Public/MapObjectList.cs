using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers.Extensions;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// <see cref="MapObjectBase"/> のリストを表します。
    /// </summary>
    /// <seealso cref="SingleStructureList"/>
    /// <seealso cref="StationList"/>
    public class MapObjectList : WrappedList<MapObjectBase>
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="MapObjectList"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected MapObjectList(IList src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="MapObjectList"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static MapObjectList FromSource(object src) => src is null ? null : new MapObjectList((IList)src);
    }
}
