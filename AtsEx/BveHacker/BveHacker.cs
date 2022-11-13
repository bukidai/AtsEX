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

using AtsEx.Handles;
using AtsEx.MapStatements;
using AtsEx.PluginHost;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.MapStatements;
using AtsEx.Scripting.CSharp;

namespace AtsEx
{
    internal sealed class BveHacker : PluginHost.BveHacker, IDisposable
    {
        private NativeImpl Native;

        public BveHacker(Action<Version> profileForDifferentBveVersionLoaded) : base(profileForDifferentBveVersionLoaded)
        {
            _ContextMenuHacker = new ContextMenuHacker(MainForm);
            _ContextMenuHacker.AddSeparator(true);

            _MapHeaders = HeaderSet.FromMap(ScenarioInfo.RouteFiles.SelectedFile.Path);
        }

        protected override void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            _MapStatements = StatementSet.Create(e.Scenario.Route.Structures.Repeated, e.Scenario.Route.StructureModels, e.Scenario.Trains);

            NotchInfo notchInfo = e.Scenario.Vehicle.Instruments.Cab.Handles.NotchInfo;

            BrakeHandle brake = BrakeHandle.FromNotchInfo(notchInfo, Native.Handles.Brake.CanSetNotchOutOfRange);
            PowerHandle power = PowerHandle.FromNotchInfo(notchInfo, Native.Handles.Power.CanSetNotchOutOfRange);
            Reverser reverser = new Reverser();

            _Handles = new PluginHost.Handles.HandleSet(brake, power, reverser);

            try
            {
                _ExtendedBeacons = global::AtsEx.ExtendedBeacons.ExtendedBeaconSet.Load(Native, this, e.Scenario.Route.Structures.Repeated, e.Scenario.Route.StructureModels, e.Scenario.Trains);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case BveFileLoadException exception:
                        LoadErrorManager.Throw(exception.Message, exception.SenderFileName, exception.LineIndex, exception.CharIndex);
                        break;

                    case CompilationException exception:
                        foreach (Diagnostic diagnostic in exception.CompilationErrors)
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
                        LoadErrorManager.Throw(ex.Message);
                        MessageBox.Show(ex.ToString(), App.Instance.ProductName);
                        break;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _ContextMenuHacker.Dispose();
        }

        public void SetScenario(NativeImpl native)
        {
            Native = native;
        }

        public void Tick(TimeSpan elapsed)
        {
            double location = Scenario.LocationManager.Location;
            int time = Scenario.TimeManager.TimeMilliseconds;
            double preTrainLocation = Scenario.Route.PreTrainObjects.GetPreTrainLocation(time);
            _ExtendedBeacons?.Tick(location, preTrainLocation);
        }


        private readonly ContextMenuHacker _ContextMenuHacker;
        public override IContextMenuHacker ContextMenuHacker => _ContextMenuHacker;


        private PluginHost.Handles.HandleSet _Handles;
        public override PluginHost.Handles.HandleSet Handles => _Handles;

        private ExtendedBeacons.ExtendedBeaconSet _ExtendedBeacons;
        public override PluginHost.ExtendedBeacons.ExtendedBeaconSet ExtendedBeacons => _ExtendedBeacons;

#pragma warning disable IDE1006 // 命名スタイル
        public HeaderSet _MapHeaders { get; }
#pragma warning restore IDE1006 // 命名スタイル
        public override IHeaderSet MapHeaders => _MapHeaders;

#pragma warning disable IDE1006 // 命名スタイル
        public IStatementSet _MapStatements { get; private set; }
#pragma warning restore IDE1006 // 命名スタイル
        public override IStatementSet MapStatements => _MapStatements;
    }
}
