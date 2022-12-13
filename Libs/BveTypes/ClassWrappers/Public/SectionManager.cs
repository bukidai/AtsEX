using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
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

            TimeManagerField = members.GetSourceFieldOf(nameof(TimeManager));
            PreTrainPassObjectsField = members.GetSourceFieldOf(nameof(PreTrainPassObjects));
            PreTrainSectionIndexField = members.GetSourceFieldOf(nameof(PreTrainSectionIndex));
            SectionIndexesTrainOnField = members.GetSourceFieldOf(nameof(SectionIndexesTrainOn));
            PreTrainLocationField = members.GetSourceFieldOf(nameof(PreTrainLocation));

            OnSectionChangedMethod = members.GetSourceMethodOf(nameof(OnSectionChanged));
            UpdatePreTrainSectionMethod = members.GetSourceMethodOf(nameof(UpdatePreTrainSection));
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

        private static FastMethod SectionsGetMethod;
        /// <summary>
        /// 閉塞の一覧を取得します。
        /// </summary>
        public MapFunctionList Sections => MapFunctionList.FromSource(SectionsGetMethod.Invoke(Src, null));

        private static FastField TimeManagerField;
        /// <summary>
        /// 閉塞の制御に使用する <see cref="ClassWrappers.TimeManager"/> を取得・設定します。
        /// </summary>
        public TimeManager TimeManager
        {
            get => TimeManager.FromSource(TimeManagerField.GetValue(Src));
            set => TimeManagerField.SetValue(Src, value.Src);
        }

        private static FastField PreTrainPassObjectsField;
        /// <summary>
        /// 先行列車の走行位置を定義している PreTrain.Pass ステートメントの一覧を取得・設定します。
        /// </summary>
        public MapObjectList PreTrainPassObjects
        {
            get => MapObjectList.FromSource(PreTrainPassObjectsField.GetValue(Src));
            set => PreTrainPassObjectsField.SetValue(Src, value.Src);
        }

        private static FastField PreTrainSectionIndexField;
        /// <summary>
        /// 先行列車が走行している閉塞のインデックスを取得・設定します。
        /// </summary>
        public int PreTrainSectionIndex
        {
            get => PreTrainSectionIndexField.GetValue(Src);
            set => PreTrainSectionIndexField.SetValue(Src, value);
        }

        private static FastField SectionIndexesTrainOnField;
        /// <summary>
        /// 列車が走行している閉塞のインデックスの一覧を取得・設定します。
        /// </summary>
        public List<int> SectionIndexesTrainOn
        {
            get => SectionIndexesTrainOnField.GetValue(Src);
            set => SectionIndexesTrainOnField.SetValue(Src, value);
        }

        private static FastField PreTrainLocationField;
        /// <summary>
        /// 先行列車が走行している距離程 [m] を取得・設定します。
        /// </summary>
        public double PreTrainLocation
        {
            get => PreTrainLocationField.GetValue(Src);
            set => PreTrainLocationField.SetValue(Src, value);
        }

        private static FastMethod OnSectionChangedMethod;
        /// <summary>
        /// 自列車または先行列車の走行している閉塞が変更されたときに呼び出されます。
        /// </summary>
        public void OnSectionChanged() => OnSectionChangedMethod.Invoke(Src, new object[] { });

        private static FastMethod UpdatePreTrainSectionMethod;
        /// <summary>
        /// 先行列車が走行している閉塞を最新の状態に更新します。
        /// </summary>
        public void UpdatePreTrainSection() => UpdatePreTrainSectionMethod.Invoke(Src, new object[] { });
    }
}
