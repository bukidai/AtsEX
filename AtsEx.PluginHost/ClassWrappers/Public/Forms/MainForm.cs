using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Mackoy.Bvets;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// メインのフォームを表します。
    /// </summary>
    public sealed class MainForm : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<MainForm>();

            ContextMenuField = members.GetSourceFieldOf(nameof(ContextMenu));

            CurrentScenarioInfoField = members.GetSourceFieldOf(nameof(CurrentScenarioInfo));
            CurrentScenarioField = members.GetSourceFieldOf(nameof(CurrentScenario));
            PreferencesField = members.GetSourceFieldOf(nameof(Preferences));
            KeyProviderField = members.GetSourceFieldOf(nameof(KeyProvider));

            ScenarioSelectFormField = members.GetSourceFieldOf(nameof(ScenarioSelectForm));
            LoadingProgressFormField = members.GetSourceFieldOf(nameof(LoadingProgressForm));
            TimePosFormField = members.GetSourceFieldOf(nameof(TimePosForm));
            ChartFormField = members.GetSourceFieldOf(nameof(ChartForm));

            LoadScenarioMethod = members.GetSourceMethodOf(nameof(LoadScenario));
            UnloadScenarioMethod = members.GetSourceMethodOf(nameof(UnloadScenario));
        }

        private MainForm(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="MainForm"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static MainForm FromSource(object src) => src is null ? null : new MainForm(src);

        private static FieldInfo ScenarioSelectFormField;
        /// <summary>
        /// 「シナリオの選択」フォームを取得します。
        /// </summary>
        public ScenarioSelectionForm ScenarioSelectForm => ClassWrappers.ScenarioSelectionForm.FromSource(ScenarioSelectFormField.GetValue(Src));

        private static FieldInfo LoadingProgressFormField;
        /// <summary>
        /// 「シナリオを読み込んでいます...」フォームを取得します。
        /// </summary>
        public LoadingProgressForm LoadingProgressForm => ClassWrappers.LoadingProgressForm.FromSource(LoadingProgressFormField.GetValue(Src));

        private static FieldInfo TimePosFormField;
        /// <summary>
        /// 「時刻と位置」フォームを取得します。
        /// </summary>
        public TimePosForm TimePosForm => ClassWrappers.TimePosForm.FromSource(TimePosFormField.GetValue(Src));

        private static FieldInfo ChartFormField;
        /// <summary>
        /// 「車両物理量」フォームを取得します。
        /// </summary>
        public ChartForm ChartForm => ClassWrappers.ChartForm.FromSource(ChartFormField.GetValue(Src));


        private static FieldInfo ContextMenuField;
        /// <summary>
        /// 右クリックで表示されるショートカット メニューを取得・設定します。
        /// </summary>
        /// <remarks>
        /// メニューの内容を編集する場合、通常は <see cref="IContextMenuHacker"/> を使用してください。
        /// </remarks>
        public ContextMenuStrip ContextMenu
        {
            get => ContextMenuField.GetValue(Src);
            set => ContextMenuField.SetValue(Src, value);
        }


        private static FieldInfo CurrentScenarioInfoField;
        /// <summary>
        /// 現在のシナリオの <see cref="ScenarioInfo"/> を取得・設定します。
        /// </summary>
        public ScenarioInfo CurrentScenarioInfo
        {
            get => ScenarioInfo.FromSource(CurrentScenarioInfoField.GetValue(Src));
            set => CurrentScenarioInfoField.SetValue(Src, value.Src);
        }

        private static FieldInfo CurrentScenarioField;
        /// <summary>
        /// 現在のシナリオのインスタンスを取得・設定します。
        /// </summary>
        public Scenario CurrentScenario
        {
            get => Scenario.FromSource(CurrentScenarioField.GetValue(Src));
            set => CurrentScenarioField.SetValue(Src, value.Src);
        }

        private static FieldInfo PreferencesField;
        /// <summary>
        /// BVE の設定が格納された <see cref="Mackoy.Bvets.Preferences"/> を取得・設定します。
        /// </summary>
        public Preferences Preferences
        {
            get => PreferencesField.GetValue(Src);
            set => PreferencesField.SetValue(Src, value);
        }

        private static FieldInfo KeyProviderField;
        /// <summary>
        /// キー入力を管理する <see cref="ClassWrappers.KeyProvider"/> を取得・設定します。
        /// </summary>
        public KeyProvider KeyProvider
        {
            get => KeyProvider.FromSource(KeyProviderField.GetValue(Src));
            set => KeyProviderField.SetValue(Src, value.Src);
        }


        private static MethodInfo LoadScenarioMethod;
        /// <summary>
        /// シナリオを読み込みます。
        /// </summary>
        /// <param name="scenarioInfo">シナリオを指定する <see cref="ScenarioInfo"/>。</param>
        /// <param name="reload">同一のシナリオの再読込であるか。<see langword="true"/> を指定した場合、現時点で読み込まれているストラクチャーを流用します。</param>
        public void LoadScenario(ScenarioInfo scenarioInfo, bool reload) => LoadScenarioMethod.Invoke(Src, new object[] { scenarioInfo, reload });

        private static MethodInfo UnloadScenarioMethod;
        /// <summary>
        /// シナリオを閉じます。
        /// </summary>
        public void UnloadScenario() => UnloadScenarioMethod.Invoke(Src, null);
    }
}
