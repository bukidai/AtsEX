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
    /// 閉塞を制御するための機能を提供します。
    /// </summary>
    public class SectionManager : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<SectionManager>();

            SectionsGetMethod = members.GetSourcePropertyGetterOf(nameof(Sections));

            SectionIndexesTrainOnField = members.GetSourceFieldOf(nameof(SectionIndexesTrainOn));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="SectionManager"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected SectionManager(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="SectionManager"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static SectionManager FromSource(object src) => src is null ? null : new SectionManager(src);

        private static MethodInfo SectionsGetMethod;
        /// <summary>
        /// 閉塞の一覧を取得します。
        /// </summary>
        public MapFunctionList Sections => MapFunctionList.FromSource(SectionsGetMethod.Invoke(Src, null));

        private static FieldInfo SectionIndexesTrainOnField;
        /// <summary>
        /// 列車が走行している閉塞のインデックスの一覧を取得・設定します。
        /// </summary>
        public List<int> SectionIndexesTrainOn
        {
            get => SectionIndexesTrainOnField.GetValue(Src);
            set => SectionIndexesTrainOnField.SetValue(Src, value);
        }
    }
}
