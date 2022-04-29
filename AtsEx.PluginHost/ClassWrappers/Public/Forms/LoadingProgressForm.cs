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
    public sealed class LoadingProgressForm : ClassWrapper
    {
        static LoadingProgressForm()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<LoadingProgressForm>();

            IsErrorCriticalField = members.GetSourceFieldOf(nameof(IsErrorCritical));
            ErrorCountField = members.GetSourceFieldOf(nameof(ErrorCount));
            PanelField = members.GetSourceFieldOf(nameof(Panel));
            ErrorListViewField = members.GetSourceFieldOf(nameof(ErrorListView));

            ThrowErrorMethod1 = members.GetSourceMethodOf(nameof(ThrowError), new Type[] { typeof(string), typeof(string), typeof(int), typeof(int) });
            ThrowErrorMethod2 = members.GetSourceMethodOf(nameof(ThrowError), new Type[] { typeof(LoadError) });
            ThrowErrorsMethod = members.GetSourceMethodOf(nameof(ThrowErrors), new Type[] { typeof(IEnumerable<LoadError>) });
        }

        private LoadingProgressForm(object src) : base(src)
        {
        }

        public static LoadingProgressForm FromSource(object src)
        {
            if (src is null) return null;
            return new LoadingProgressForm(src);
        }


        private static FieldInfo IsErrorCriticalField;
        public bool IsErrorCritical
        {
            get => IsErrorCriticalField.GetValue(Src);
            set => IsErrorCriticalField.SetValue(Src, value);
        }

        private static FieldInfo ErrorCountField;
        internal int ErrorCount
        {
            get => ErrorCountField.GetValue(Src);
            set => ErrorCountField.SetValue(Src, value);
        }

        private static FieldInfo PanelField;
        internal Panel Panel => PanelField.GetValue(Src);

        private static FieldInfo ErrorListViewField;
        internal ListView ErrorListView => ErrorListViewField.GetValue(Src);


        private static MethodInfo ThrowErrorMethod1;
        public void ThrowError(string text, string senderFileName, int lineIndex, int charIndex)
            => ThrowErrorMethod1.Invoke(Src, new object[] { text, senderFileName, lineIndex, charIndex });

        private static MethodInfo ThrowErrorMethod2;
        public void ThrowError(LoadError error) => ThrowErrorMethod2.Invoke(Src, new object[] { error.Src });

        private static MethodInfo ThrowErrorsMethod;
        public void ThrowErrors(IEnumerable<LoadError> errors) => ThrowErrorsMethod.Invoke(Src, new object[] { errors.Select(error => error.Src) });
    }
}
