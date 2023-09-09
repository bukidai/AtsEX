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
    /// カメラの位置に関する情報を提供します。
    /// </summary>
    public class CameraLocation : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<CameraLocation>();
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="CameraLocation"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected CameraLocation(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="CameraLocation"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static CameraLocation FromSource(object src) => src is null ? null : new CameraLocation(src);
    }
}
