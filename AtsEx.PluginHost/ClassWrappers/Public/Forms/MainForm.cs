using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.Helpers;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// メインのフォームを表します。
    /// </summary>
    public sealed class MainForm : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<MainForm>();

            ContextMenuField = members.GetSourceFieldOf(nameof(ContextMenu));

            CurrentScenarioInfoField = members.GetSourceFieldOf(nameof(CurrentScenarioInfo));
            CurrentScenarioField = members.GetSourceFieldOf(nameof(CurrentScenario));

            ScenarioSelectFormField = members.GetSourceFieldOf(nameof(ScenarioSelectForm));
            LoadingProgressFormField = members.GetSourceFieldOf(nameof(LoadingProgressForm));
            TimePosFormField = members.GetSourceFieldOf(nameof(TimePosForm));
            ChartFormField = members.GetSourceFieldOf(nameof(ChartForm));
        }

        private MainForm(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="MainForm"/> クラスのインスタンス。</returns>
        public static MainForm FromSource(object src)
        {
            if (src is null) return null;
            return new MainForm(src);
        }

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
        /// 右クリックで表示されるショートカット メニューを取得します。
        /// </summary>
        /// <remarks>
        /// メニューの内容を編集する場合、通常は <see cref="ContextMenuHacker"/> を使用してください。
        /// </remarks>
        public ContextMenuStrip ContextMenu
        {
            get => ContextMenuField.GetValue(Src);
            set => ContextMenuField.SetValue(Src, value);
        }


        private static FieldInfo CurrentScenarioInfoField;
        /// <summary>
        /// 現在のシナリオの <see cref="ScenarioInfo"/> を取得します。
        /// </summary>
        public ScenarioInfo CurrentScenarioInfo
        {
            get => ScenarioInfo.FromSource(CurrentScenarioInfoField.GetValue(Src));
            set => CurrentScenarioInfoField.SetValue(Src, value);
        }

        private static FieldInfo CurrentScenarioField;
        /// <summary>
        /// 現在のシナリオのインスタンスを取得します。
        /// </summary>
        public Scenario CurrentScenario
        {
            get => Scenario.FromSource(CurrentScenarioField.GetValue(Src));
            set => CurrentScenarioField.SetValue(Src, value);
        }
    }
}
