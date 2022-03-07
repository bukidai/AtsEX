using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal interface ILoadingProgressForm : IClassWrapper
    {
        void ThrowError(string text, string senderFileName, int lineIndex, int charIndex);
        void ThrowError(ILoadError error);
        void ThrowErrors(IEnumerable<ILoadError> errors);
    }

    internal class LoadingProgressForm : ClassWrapper, ILoadingProgressForm
    {
        public LoadingProgressForm(object src) : base(src)
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<ILoadingProgressForm>();

            ThrowErrorMethod1 = members.GetSourceMethodOf(nameof(ThrowError), new Type[] { typeof(string), typeof(string), typeof(int), typeof(int) });
            ThrowErrorMethod2 = members.GetSourceMethodOf(nameof(ThrowError), new Type[] { typeof(ILoadError) });
            ThrowErrorsMethod = members.GetSourceMethodOf(nameof(ThrowErrors), new Type[] { typeof(IEnumerable<ILoadError>) });
        }

        private MethodInfo ThrowErrorMethod1;
        public void ThrowError(string text, string senderFileName, int lineIndex, int charIndex)
            => ThrowErrorMethod1.Invoke(Src, new object[] { text, senderFileName, lineIndex, charIndex });

        private MethodInfo ThrowErrorMethod2;
        public void ThrowError(ILoadError error) => ThrowErrorMethod2.Invoke(Src, new object[] { error.Src });

        private MethodInfo ThrowErrorsMethod;
        public void ThrowErrors(IEnumerable<ILoadError> errors) => ThrowErrorsMethod.Invoke(Src, new object[] { errors.Select(error => error.Src) });
    }
}
