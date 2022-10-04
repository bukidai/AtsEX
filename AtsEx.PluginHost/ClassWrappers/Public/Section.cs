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
    /// 閉塞を表します。
    /// </summary>
    public class Section : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<Section>();

            CurrentSignalIndexGetMethod = members.GetSourcePropertyGetterOf(nameof(CurrentSignalIndex));

            SignalIndexesGetMethod = members.GetSourcePropertyGetterOf(nameof(SignalIndexes));
            SignalIndexesSetMethod = members.GetSourcePropertySetterOf(nameof(SignalIndexes));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="Section"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected Section(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Section"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static Section FromSource(object src) => src is null ? null : new Section(src);

        private static FastMethod CurrentSignalIndexGetMethod;
        /// <summary>
        /// 現在の信号現示のインデックスを取得します。
        /// </summary>
        public int CurrentSignalIndex => CurrentSignalIndexGetMethod.Invoke(Src, null);

        private static FastMethod SignalIndexesGetMethod;
        private static FastMethod SignalIndexesSetMethod;
        /// <summary>
        /// 信号現示インデックスの一覧を取得・設定します。
        /// </summary>
        public int[] SignalIndexes
        {
            get => SignalIndexesGetMethod.Invoke(Src, null);
            set => SignalIndexesSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
