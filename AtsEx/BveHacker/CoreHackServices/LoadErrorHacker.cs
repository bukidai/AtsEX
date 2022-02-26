using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.ClassWrappers;

namespace Automatic9045.AtsEx.CoreHackServices
{
    internal interface ILoadErrorHacker
    {
        void ThrowError(string text, string senderFileName, int lineIndex = 0, int charIndex = 0);
        void ThrowError(LoadError error);
        void ThrowError(IEnumerable<LoadError> errors);
    }

    internal sealed class LoadErrorHacker : CoreHackService, ILoadErrorHacker
    {
        private Form LoadingProgressForm;
        private Type LoadingProgressFormType;

        [WillRefactor] // BVE独自型は静的リストにする予定
        private Type LoadErrorType;

        [UnderConstruction]
        public LoadErrorHacker(Process targetProcess, Assembly targetAssembly, ServiceCollection services) : base(targetProcess, targetAssembly, services)
        {
            LoadingProgressForm = services.GetService<ISubFormHacker>().LoadingProgressForm;
            LoadingProgressFormType = LoadingProgressForm.GetType();

            LoadErrorType = TargetAssembly.GetType("ai");

            ThrowErrorMethod1 = LoadingProgressFormType.GetMethod("a", new Type[] { typeof(string), typeof(string), typeof(int), typeof(int) });
            ThrowErrorMethod2 = LoadingProgressFormType.GetMethod("a", new Type[] { LoadErrorType });
            ThrowErrorMethod3 = LoadingProgressFormType.GetMethod("a", new Type[] { typeof(IEnumerable<>).MakeGenericType(LoadErrorType) });
        }

        private MethodInfo ThrowErrorMethod1;
        public void ThrowError(string text, string senderFileName, int lineIndex, int charIndex)
            => ThrowErrorMethod1.Invoke(LoadingProgressForm, new object[] { text, senderFileName, lineIndex, charIndex });

        private MethodInfo ThrowErrorMethod2;
        public void ThrowError(LoadError error) => ThrowErrorMethod2.Invoke(LoadingProgressForm, new object[] { error });

        private MethodInfo ThrowErrorMethod3;
        public void ThrowError(IEnumerable<LoadError> errors) => ThrowErrorMethod3.Invoke(LoadingProgressForm, new object[] { errors });
    }
}
