using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

using AtsEx.PluginHost.BveTypes;

namespace AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 自列車がマップ オブジェクトを通過したときに発生するイベントのデータを提供します。
    /// </summary>
    public class ObjectPassedEventArgs : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<ObjectPassedEventArgs>();

            MapObjectGetMethod = members.GetSourcePropertyGetterOf(nameof(MapObject));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="ObjectPassedEventArgs"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected ObjectPassedEventArgs(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="ObjectPassedEventArgs"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static ObjectPassedEventArgs FromSource(object src) => src is null ? null : new ObjectPassedEventArgs(src);

        private static FastMethod MapObjectGetMethod;
        /// <summary>
        /// 通過したマップ オブジェクトを取得します。
        /// </summary>
        public MapObjectBase MapObject => (MapObjectBase)CreateFromSource(MapObjectGetMethod.Invoke(this, null));
    }
}
