using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AtsEx.Native;
using AtsEx.PluginHost;

namespace AtsEx
{
    internal abstract partial class AtsEx
    {
        internal sealed class AsAtsPlugin : AtsEx
        {
            private const string LegalLauncherAssemblyRelativeLocation = @"AtsEx\AtsEx.Launcher.dll";

            public string VersionWarningText { get; private set; }

            public AsAtsPlugin(CallerInfo callerInfo)
                : base(callerInfo)
            {
                CheckAtsExAssemblyLocation();
            }

            private void CheckAtsExAssemblyLocation()
            {
                string launcherAssemblyLocation = App.Instance.AtsExLauncherAssembly.Location;

                string scenarioDirectory = BveHacker.ScenarioInfo.DirectoryName;
                string legalLauncherAssemblyLocation = Path.Combine(scenarioDirectory, LegalLauncherAssemblyRelativeLocation);

                if (GetNormalizedPath(launcherAssemblyLocation) != GetNormalizedPath(legalLauncherAssemblyLocation))
                {
                    string warningText = string.Format(Resources.Value.AtsExAssemblyLocationIllegal.Value,
                        App.Instance.ProductShortName, launcherAssemblyLocation, legalLauncherAssemblyLocation);

                    BveHacker.LoadErrorManager.Throw(warningText.Replace("\n", ""), Path.GetFileName(launcherAssemblyLocation));
                    if (MessageBox.Show($"{warningText}\n\n{Resources.Value.IgnoreAndContinue.Value}", App.Instance.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        Environment.Exit(0);
                    }
                }


                string GetNormalizedPath(string abdolutePath) => Path.GetFullPath(abdolutePath).ToLower();
            }

            protected override void ProfileForDifferentBveVersionLoaded(Version profileVersion)
            {
                VersionWarningText = string.Format(Resources.Value.BveVersionNotSupported.Value, App.Instance.BveVersion, profileVersion, App.Instance.ProductShortName);
                BveHacker.LoadErrorManager.Throw(VersionWarningText);
            }
        }
    }
}
