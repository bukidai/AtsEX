using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    public abstract partial class AtsExExtensionSet
    {
        public sealed class AsAtsPlugin : AtsExExtensionSet
        {
            internal string VersionWarningText { get; private set; }

            public AsAtsPlugin(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly) : base(targetProcess, targetAppDomain, targetAssembly, new LoadErrorResolver())
            {
                (base.LoadErrorResolver as LoadErrorResolver).LoadErrorManager = BveHacker.LoadErrorManager;
            }

            private protected override void ProfileForDifferentBveVersionLoaded(Version profileVersion)
            {
                VersionWarningText = string.Format(Resources.GetString("BveVersionNotSupported").Value, App.Instance.BveVersion, profileVersion, App.Instance.ProductShortName);
                BveHacker.LoadErrorManager.Throw(VersionWarningText);
            }


            private new class LoadErrorResolver : ILoadErrorResolver
            {
                public LoadErrorManager LoadErrorManager { get; set; }

                public LoadErrorResolver()
                {
                }

                public void Resolve(Exception exception)
                {
                    if (exception is BveFileLoadException fe)
                    {
                        LoadErrorManager.Throw(fe.Message, fe.SenderFileName, fe.LineIndex, fe.CharIndex);
                    }
                    else
                    {
                        LoadErrorManager.Throw(exception.Message);
                        MessageBox.Show(exception.ToString(), string.Format(Resources.GetString("UnhandledExceptionCaption").Value, App.Instance.ProductShortName));
                    }
                }
            }
        }
    }
}
