using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal interface IMainForm : IClassWrapper
    {
        ContextMenuStrip ContextMenu { get; set; }

        IScenarioInfo CurrentScenarioInfo { get; set; }
        IScenarioProvider CurrentScenarioProvider { get; set; }
    }

    internal class MainForm : ClassWrapper, IMainForm
    {
        public MainForm(object src) : base(src)
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IMainForm>();

            ContextMenuField = members.GetSourceFieldOf(nameof(ContextMenu));

            CurrentScenarioInfoField = members.GetSourceFieldOf(nameof(CurrentScenarioInfo));
            CurrentScenarioProviderField = members.GetSourceFieldOf(nameof(CurrentScenarioProvider));
        }

        private FieldInfo ContextMenuField;
        public ContextMenuStrip ContextMenu
        {
            get => ContextMenuField.GetValue(Src);
            set => ContextMenuField.SetValue(Src, value);
        }


        private FieldInfo CurrentScenarioInfoField;
        public IScenarioInfo CurrentScenarioInfo
        {
            get => new ScenarioInfo(CurrentScenarioInfoField.GetValue(Src));
            set => CurrentScenarioInfoField.SetValue(Src, value);
        }

        private FieldInfo CurrentScenarioProviderField;
        public IScenarioProvider CurrentScenarioProvider
        {
            get => new ScenarioProvider(CurrentScenarioProviderField.GetValue(Src));
            set => CurrentScenarioProviderField.SetValue(Src, value);
        }
    }
}
