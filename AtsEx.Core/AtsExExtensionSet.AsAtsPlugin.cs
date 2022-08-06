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
using Automatic9045.AtsEx.PluginHost.Helpers;

namespace Automatic9045.AtsEx
{
    public abstract partial class AtsExExtensionSet
    {
        public sealed class AsAtsPlugin : AtsExExtensionSet
        {
            internal string VersionWarningText { get; private set; }

            public AsAtsPlugin(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly) : base(targetProcess, targetAppDomain, targetAssembly)
            {
            }

            private protected override void ProfileForDifferentBveVersionLoaded(Version profileVersion)
            {
                VersionWarningText = string.Format(Resources.GetString("BveVersionNotSupported").Value, App.Instance.BveVersion, profileVersion, App.Instance.ProductShortName);
                LoadErrorManager.Throw(VersionWarningText);
            }

            private protected override void ResolveLoadException(Exception exception)
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
