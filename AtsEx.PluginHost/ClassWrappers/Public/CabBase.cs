using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 運転台のハンドルを表します。
    /// </summary>
    public class CabBase : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<CabBase>();

            HandlesGetMethod = members.GetSourcePropertyGetterOf(nameof(Handles));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="CabBase"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected CabBase(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="CabBase"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static CabBase FromSource(object src) => src is null ? null : new CabBase(src);

        private static FastMethod HandlesGetMethod;
        /// <summary>
        /// 操作可能なハンドルのセットを表す <see cref="HandleSet"/> を取得します。
        /// </summary>
        public HandleSet Handles => HandleSet.FromSource(HandlesGetMethod.Invoke(Src, null));
    }
}
