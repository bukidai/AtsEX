using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal sealed class LoadingProgressForm : ClassWrapper
    {
        static LoadingProgressForm()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<LoadingProgressForm>();

            ThrowErrorMethod1 = members.GetSourceMethodOf(nameof(ThrowError), new Type[] { typeof(string), typeof(string), typeof(int), typeof(int) });
            ThrowErrorMethod2 = members.GetSourceMethodOf(nameof(ThrowError), new Type[] { typeof(LoadError) });
            ThrowErrorsMethod = members.GetSourceMethodOf(nameof(ThrowErrors), new Type[] { typeof(IEnumerable<LoadError>) });
        }

        public LoadingProgressForm(object src) : base(src)
        {
        }

        private static MethodInfo ThrowErrorMethod1;
        public void ThrowError(string text, string senderFileName, int lineIndex, int charIndex)
            => ThrowErrorMethod1.Invoke(Src, new object[] { text, senderFileName, lineIndex, charIndex });

        private static MethodInfo ThrowErrorMethod2;
        public void ThrowError(LoadError error) => ThrowErrorMethod2.Invoke(Src, new object[] { error.Src });

        private static MethodInfo ThrowErrorsMethod;
        public void ThrowErrors(IEnumerable<LoadError> errors) => ThrowErrorsMethod.Invoke(Src, new object[] { errors.Select(error => error.Src) });
    }
}
