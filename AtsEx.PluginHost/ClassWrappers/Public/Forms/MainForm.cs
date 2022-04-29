using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class MainForm : ClassWrapper
    {
        static MainForm()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<MainForm>();

            ContextMenuField = members.GetSourceFieldOf(nameof(ContextMenu));

            CurrentScenarioInfoField = members.GetSourceFieldOf(nameof(CurrentScenarioInfo));
            CurrentScenarioProviderField = members.GetSourceFieldOf(nameof(CurrentScenarioProvider));

            ScenarioSelectFormField = members.GetSourceFieldOf(nameof(ScenarioSelectForm));
            LoadingProgressFormField = members.GetSourceFieldOf(nameof(LoadingProgressForm));
            TimePosFormField = members.GetSourceFieldOf(nameof(TimePosForm));
            ChartFormField = members.GetSourceFieldOf(nameof(ChartForm));
        }

        private MainForm(object src) : base(src)
        {
        }

        public static MainForm FromSource(object src)
        {
            if (src is null) return null;
            return new MainForm(src);
        }

        private static FieldInfo ScenarioSelectFormField;
        public ScenarioSelectionForm ScenarioSelectForm => ClassWrappers.ScenarioSelectionForm.FromSource(ScenarioSelectFormField.GetValue(Src));

        private static FieldInfo LoadingProgressFormField;
        public LoadingProgressForm LoadingProgressForm => ClassWrappers.LoadingProgressForm.FromSource(LoadingProgressFormField.GetValue(Src));

        private static FieldInfo TimePosFormField;
        public TimePosForm TimePosForm => ClassWrappers.TimePosForm.FromSource(TimePosFormField.GetValue(Src));

        private static FieldInfo ChartFormField;
        public ChartForm ChartForm => ClassWrappers.ChartForm.FromSource(ChartFormField.GetValue(Src));


        private static FieldInfo ContextMenuField;
        public ContextMenuStrip ContextMenu
        {
            get => ContextMenuField.GetValue(Src);
            set => ContextMenuField.SetValue(Src, value);
        }


        private static FieldInfo CurrentScenarioInfoField;
        public ScenarioInfo CurrentScenarioInfo
        {
            get => ScenarioInfo.FromSource(CurrentScenarioInfoField.GetValue(Src));
            set => CurrentScenarioInfoField.SetValue(Src, value);
        }

        private static FieldInfo CurrentScenarioProviderField;
        public ScenarioProvider CurrentScenarioProvider
        {
            get => ScenarioProvider.FromSource(CurrentScenarioProviderField.GetValue(Src));
            set => CurrentScenarioProviderField.SetValue(Src, value);
        }
    }
}
