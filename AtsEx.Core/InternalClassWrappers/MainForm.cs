using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal sealed class MainForm : ClassWrapper
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

        public static MainForm FromSource(object src) => new MainForm(src);

        private static FieldInfo ScenarioSelectFormField;
        public Form ScenarioSelectForm
        {
            get => ScenarioSelectFormField.GetValue(Src);
        }

        private static FieldInfo LoadingProgressFormField;
        public Form LoadingProgressForm
        {
            get => LoadingProgressFormField.GetValue(Src);
        }

        private static FieldInfo TimePosFormField;
        public Form TimePosForm
        {
            get => TimePosFormField.GetValue(Src);
        }

        private static FieldInfo ChartFormField;
        public Form ChartForm
        {
            get => ChartFormField.GetValue(Src);
        }


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
