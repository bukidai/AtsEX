﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.CoreHackServices
{
    internal interface ILoadErrorHacker
    {
        void ThrowError(string text, string senderFileName, int lineIndex = 0, int charIndex = 0);
        void ThrowError(LoadError error);
        void ThrowErrors(IEnumerable<LoadError> errors);
    }

    internal sealed class LoadErrorHacker : CoreHackService, ILoadErrorHacker
    {
        private LoadingProgressForm LoadingProgressForm;

        public LoadErrorHacker(Process targetProcess, ServiceCollection services) : base(targetProcess, services)
        {
            Form formSource = services.GetService<ISubFormHacker>().LoadingProgressForm;
            LoadingProgressForm = LoadingProgressForm.FromSource(formSource);
        }

        public void ThrowError(string text, string senderFileName, int lineIndex, int charIndex) => LoadingProgressForm.ThrowError(text, senderFileName, lineIndex, charIndex);

        public void ThrowError(LoadError error) => LoadingProgressForm.ThrowError(error);

        public void ThrowErrors(IEnumerable<LoadError> errors) => LoadingProgressForm.ThrowErrors(errors);
    }
}
