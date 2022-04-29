using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.Helpers
{
    public static class LoadErrorManager
    {
        public static void ThrowError(string text, string senderFileName, int lineIndex, int charIndex)
        {
            InstanceStore.BveHacker.LoadingProgressForm.ThrowError(text, senderFileName, lineIndex, charIndex);
        }

        public static void ThrowError(string text, string senderFileName, int lineIndex) => ThrowError(text, senderFileName, lineIndex, 0);

        public static void ThrowError(string text, string senderFileName) => ThrowError(text, senderFileName, 0);

        public static void ThrowError(string text) => ThrowError(text, "");

        public static void ThrowError(LoadError error)
        {
            InstanceStore.BveHacker.LoadingProgressForm.ThrowError(error);
        }
    }
}
