using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal class TimeTable : AssistantText, ITimeTable
    {
        static TimeTable()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<ITimeTable>();

            ModelField = members.GetSourceFieldOf(nameof(Model));
            NameTextsField = members.GetSourceFieldOf(nameof(NameTexts));
            ArrivalTimeTextsField = members.GetSourceFieldOf(nameof(ArrivalTimeTexts));
            DepertureTimeTextsField = members.GetSourceFieldOf(nameof(DepertureTimeTexts));
            NameTextWidthsField = members.GetSourceFieldOf(nameof(NameTextWidths));
            ArrivalTimeTextWidthsField = members.GetSourceFieldOf(nameof(ArrivalTimeTextWidths));
            DepertureTimeTextWidthsField = members.GetSourceFieldOf(nameof(DepertureTimeTextWidths));

            UpdateMethod = members.GetSourceMethodOf(nameof(Update));
        }

        public TimeTable(object src) : base(src)
        {
		}

        protected static FieldInfo ModelField;
        public IModel Model
        {
            get => ModelField.GetValue(Src);
            set => ModelField.SetValue(Src, value);
        }

        protected static FieldInfo NameTextsField;
        public string[] NameTexts
        {
            get => NameTextsField.GetValue(Src);
            set => NameTextsField.SetValue(Src, value);
        }

        protected static FieldInfo ArrivalTimeTextsField;
        public string[] ArrivalTimeTexts
        {
            get => ArrivalTimeTextsField.GetValue(Src);
            set => ArrivalTimeTextsField.SetValue(Src, value);
        }

        protected static FieldInfo DepertureTimeTextsField;
        public string[] DepertureTimeTexts
        {
            get => DepertureTimeTextsField.GetValue(Src);
            set => DepertureTimeTextsField.SetValue(Src, value);
        }

        protected static FieldInfo NameTextWidthsField;
        public int[] NameTextWidths
        {
            get => NameTextWidthsField.GetValue(Src);
            set => NameTextWidthsField.SetValue(Src, value);
        }

        protected static FieldInfo ArrivalTimeTextWidthsField;
        public int[] ArrivalTimeTextWidths
        {
            get => ArrivalTimeTextWidthsField.GetValue(Src);
            set => ArrivalTimeTextWidthsField.SetValue(Src, value);
        }

        protected static FieldInfo DepertureTimeTextWidthsField;
        public int[] DepertureTimeTextWidths
        {
            get => DepertureTimeTextWidthsField.GetValue(Src);
            set => DepertureTimeTextWidthsField.SetValue(Src, value);
        }

        protected static MethodInfo UpdateMethod;
        public void Update()
        {
            UpdateMethod.Invoke(Src, null);
        }
    }
}
