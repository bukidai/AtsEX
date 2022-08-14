using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.Plugins.Scripting.CSharp;

namespace Automatic9045.AtsEx
{
    public abstract partial class AtsEx
    {
        public sealed class AsAtsPlugin : AtsEx
        {
            private const string LegalAtsExAssemblyRelativeLocation = @"Automatic9045\AtsEx\AtsEx.dll";

            internal string VersionWarningText { get; private set; }

            public AsAtsPlugin(Process targetProcess, AppDomain targetAppDomain, Assembly targetAssembly)
                : base(targetProcess, targetAppDomain, targetAssembly, new LoadErrorResolver())
            {
                (BeaconCreationExceptionResolver as LoadErrorResolver).LoadErrorManager = BveHacker.LoadErrorManager;

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

            private protected override void ProfileForDifferentBveVersionLoaded(Version profileVersion)
            {
                VersionWarningText = string.Format(Resources.GetString("BveVersionNotSupported").Value, App.Instance.BveVersion, profileVersion, App.Instance.ProductShortName);
                BveHacker.LoadErrorManager.Throw(VersionWarningText);
            }


            private class LoadErrorResolver : ILoadErrorResolver
            {
                public LoadErrorManager LoadErrorManager { get; set; }

                public LoadErrorResolver()
                {
                }

                public void Resolve(Exception exception)
                {
                    switch (exception)
                    {
                        case BveFileLoadException ex:
                            LoadErrorManager.Throw(ex.Message, ex.SenderFileName, ex.LineIndex, ex.CharIndex);
                            break;

                        case CompilationException ex:
                            foreach (Diagnostic diagnostic in ex.CompilationErrors)
                            {
                                string message = diagnostic.GetMessage();
                                string fileName = Path.GetFileName(diagnostic.Location.SourceTree.FilePath);

                                LinePosition position = diagnostic.Location.GetLineSpan().StartLinePosition;
                                int lineIndex = position.Line;
                                int charIndex = position.Character;

                                LoadErrorManager.Throw(message, fileName, lineIndex, charIndex);
                            }
                            break;

                        default:
                            LoadErrorManager.Throw(exception.Message);
                            MessageBox.Show(exception.ToString(), string.Format(Resources.GetString("UnhandledExceptionCaption").Value, App.Instance.ProductShortName));
                            break;
                    }
                }
            }
        }
    }
}
