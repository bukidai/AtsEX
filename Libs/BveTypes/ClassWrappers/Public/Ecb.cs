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
    /// 電気指令式ブレーキを表します。
    /// </summary>
    public class Ecb : BrakeControllerBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<Ecb>();
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="Ecb"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected Ecb(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Ecb"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static Ecb FromSource(object src) => src is null ? null : new Ecb(src);
    }
}
