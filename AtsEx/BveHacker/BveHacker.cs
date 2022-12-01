using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.Handles;
using AtsEx.MapStatements;

using AtsEx.PluginHost;
using AtsEx.PluginHost.LoadErrorManager;
using AtsEx.PluginHost.MapStatements;

namespace AtsEx
{
    internal sealed class BveHacker : PluginHost.BveHacker, IDisposable
    {
        public BveHacker(Action<Version> profileForDifferentBveVersionLoaded) : base(profileForDifferentBveVersionLoaded)
        {
            LoadErrorManager = new LoadErrorManager.LoadErrorManager(LoadingProgressForm);
            _MapHeaders = HeaderSet.FromMap(ScenarioInfo.RouteFiles.SelectedFile.Path);
        }

        protected override void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            _MapStatements = StatementSet.Create(e.Scenario.Route.Structures.Repeated, e.Scenario.Route.StructureModels, e.Scenario.Trains);

            NotchInfo notchInfo = e.Scenario.Vehicle.Instruments.Cab.Handles.NotchInfo;

            BrakeHandle brake = BrakeHandle.FromNotchInfo(notchInfo);
            PowerHandle power = PowerHandle.FromNotchInfo(notchInfo);
            Reverser reverser = new Reverser();

            _Handles = new PluginHost.Handles.HandleSet(brake, power, reverser);

        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Tick(TimeSpan elapsed)
        {
        }

        public override ILoadErrorManager LoadErrorManager { get; }

        private PluginHost.Handles.HandleSet _Handles;
        public override PluginHost.Handles.HandleSet Handles => _Handles;

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
