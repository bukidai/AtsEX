using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HarmonyLib;

using Mackoy.Bvets;

using Automatic9045.AtsEx.ExtendedBeacons;
using Automatic9045.AtsEx.Handles;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx
{
    internal sealed class BveHacker : PluginHost.BveHacker, IDisposable
    {
        private readonly ILoadErrorResolver BeaconCreationExceptionResolver;
        private readonly VersionFormProvider VersionFormProvider;

        public BveHacker(Action<Version> profileForDifferentBveVersionLoaded, ILoadErrorResolver beaconCreationExceptionResolver) : base(App.Instance, profileForDifferentBveVersionLoaded)
        {
            BeaconCreationExceptionResolver = beaconCreationExceptionResolver;

            _ContextMenuHacker = new ContextMenuHacker(MainForm);
            _ContextMenuHacker.AddSeparator(true);

            VersionFormProvider = new VersionFormProvider(this);
        }

        protected override void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            NotchInfo notchInfo = e.Scenario.Vehicle.Instruments.Cab.Handles.NotchInfo;

            BrakeHandle brake = BrakeHandle.FromNotchInfo(notchInfo, App.Instance.Handles.Brake.CanSetNotchOutOfRange);
            PowerHandle power = PowerHandle.FromNotchInfo(notchInfo, App.Instance.Handles.Power.CanSetNotchOutOfRange);
            Reverser reverser = new Reverser();

            _Handles = new PluginHost.Handles.HandleSet(brake, power, reverser);
            App.Instance.Handles = Handles;

            try
            {
                _ExtendedBeacons = ExtendedBeaconSet.Load(this, e.Scenario.Route.Structures.Repeated, e.Scenario.Route.StructureModels, e.Scenario.Trains);
            }
            catch (Exception ex)
            {
                BeaconCreationExceptionResolver.Resolve(ex);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            VersionFormProvider.Dispose();
            _ContextMenuHacker.Dispose();
        }

        public void SetScenario()
        {
            VersionFormProvider.Intialize(Enumerable.Concat(App.Instance.VehiclePlugins, App.Instance.MapPlugins));
        }

        [UnderConstruction]
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


        private ExtendedBeaconSet _ExtendedBeacons;
        public override IExtendedBeaconSet ExtendedBeacons => _ExtendedBeacons;
    }
}
