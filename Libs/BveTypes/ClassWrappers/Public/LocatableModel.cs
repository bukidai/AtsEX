using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using SlimDX.Direct3D9;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// 設置位置の情報を伴う 3D モデルを表します。
    /// </summary>
    public class LocatableModel : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<LocatableModel>();

            ModelGetMethod = members.GetSourcePropertyGetterOf(nameof(Model));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="LocatableModel"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected LocatableModel(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="LocatableModel"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static LocatableModel FromSource(object src) => src is null ? null : new LocatableModel(src);

        private static FastMethod ModelGetMethod;
        /// <summary>
        /// ソースとなる 3D モデルを取得します。
        /// </summary>
        public Model Model => ClassWrappers.Model.FromSource(ModelGetMethod.Invoke(Src, null));
    }
}
