using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx
{
    internal abstract partial class AtsEx
    {
        internal sealed class AsAtsPlugin : AtsEx
        {
            private const string LegalAtsExAssemblyRelativeLocation = @"Automatic9045\AtsEx\AtsEx.dll";

            public string VersionWarningText { get; private set; }

            public AsAtsPlugin(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly)
                : base(targetProcess, targetAppDomain, targetAssembly)
            {
                CheckAtsExAssemblyLocation();
            }

            private void CheckAtsExAssemblyLocation()
            {
                string atsExAssemblyLocation = App.Instance.AtsExAssembly.Location;

                string scenarioDirectory = BveHacker.ScenarioInfo.DirectoryName;
                string legalAtsExAssemblyLocation = Path.Combine(scenarioDirectory, LegalAtsExAssemblyRelativeLocation);

                if (GetNormalizedPath(atsExAssemblyLocation) != GetNormalizedPath(legalAtsExAssemblyLocation))
                {
                    string warningText = string.Format(Resources.GetString("AtsExAssemblyLocationIllegal").Value,
                        App.Instance.ProductShortName, atsExAssemblyLocation, legalAtsExAssemblyLocation);

                    BveHacker.LoadErrorManager.Throw(warningText.Replace("\n", ""), Path.GetFileName(atsExAssemblyLocation));
                    if (MessageBox.Show($"{warningText}\n\n{Resources.GetString("IgnoreAndContinue").Value}", App.Instance.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        Environment.Exit(0);
                    }
                }


                string GetNormalizedPath(string abdolutePath) => Path.GetFullPath(abdolutePath).ToLower();
            }

            protected override void ProfileForDifferentBveVersionLoaded(Version profileVersion)
            {
                VersionWarningText = string.Format(Resources.GetString("BveVersionNotSupported").Value, App.Instance.BveVersion, profileVersion, App.Instance.ProductShortName);
                BveHacker.LoadErrorManager.Throw(VersionWarningText);
            }
        }
    }
}
