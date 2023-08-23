using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// 空気ばねのモデルを表します。
    /// </summary>
    public class AirSpring : Spring
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<AirSpring>();
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="AirSpring"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected AirSpring(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="AirSpring"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static new AirSpring FromSource(object src) => src is null ? null : new AirSpring(src);
    }
}
