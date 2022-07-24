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
using Automatic9045.AtsEx.PluginHost.Handles;

namespace Automatic9045.AtsEx
{
    internal sealed class BveHacker : IBveHacker
    {
        public BveHacker(Process targetProcess, Action<Exception> resolveBeaconCreationExceptionAction)
        {
            Process = targetProcess;

            MainFormHacker = new MainFormHacker(Process);
            ScenarioHacker = new ScenarioHacker(MainFormHacker);

            ScenarioHacker.ScenarioCreated += e => PreviewScenarioCreated?.Invoke(e);
            ScenarioHacker.ScenarioCreated += e =>
            {
                NotchInfo notchInfo = e.Scenario.Vehicle.Instruments.Cab.Handles.NotchInfo;

                BrakeHandle brake = BrakeHandle.FromNotchInfo(notchInfo);
                PowerHandle power = PowerHandle.FromNotchInfo(notchInfo);
                Reverser reverser = new Reverser();

                Handles = new PluginHost.Handles.HandleSet(brake, power, reverser);
                App.Instance.Handles = Handles;

                try
                {
                    _ExtendedBeacons = ExtendedBeaconSet.Load(this, e.Scenario.Route.Structures.Repeated, e.Scenario.Route.StructureModels, e.Scenario.Trains);
                }
                catch (Exception ex)
                {
                    resolveBeaconCreationExceptionAction(ex);
                }
            };
            ScenarioHacker.ScenarioCreated += e => ScenarioCreated?.Invoke(e);
        }


        public Process Process { get; }


        private readonly MainFormHacker MainFormHacker;

        public IntPtr MainFormHandle => MainFormHacker.TargetFormHandle;
        public Form MainFormSource => MainFormHacker.TargetFormSource;
        public MainForm MainForm => MainFormHacker.TargetForm;


        public Form ScenarioSelectionFormSource => MainForm.ScenarioSelectForm.Src;
        public ScenarioSelectionForm ScenarioSelectionForm => MainForm.ScenarioSelectForm;

        public Form LoadingProgressFormSource => MainForm.LoadingProgressForm.Src;
        public LoadingProgressForm LoadingProgressForm => MainForm.LoadingProgressForm;

        public Form TimePosFormSource => MainForm.TimePosForm.Src;
        public TimePosForm TimePosForm => MainForm.TimePosForm;

        public Form ChartFormSource => MainForm.ChartForm.Src;
        public ChartForm ChartForm => MainForm.ChartForm;


        public Preferences Preferences
        {
            get => MainForm.Preferences;
            set => MainForm.Preferences = value;
        }

        public KeyProvider KeyProvider
        {
            get => MainForm.KeyProvider;
            set => MainForm.KeyProvider = value;
        }


        public PluginHost.Handles.HandleSet Handles { get; private set; }

        private ExtendedBeaconSet _ExtendedBeacons;
        public IExtendedBeaconSet ExtendedBeacons => _ExtendedBeacons;


        private readonly ScenarioHacker ScenarioHacker;

        public event ScenarioCreatedEventHandler PreviewScenarioCreated;
        public event ScenarioCreatedEventHandler ScenarioCreated;

        public ScenarioInfo ScenarioInfo
        {
            get => ScenarioHacker.CurrentScenarioInfo;
            set => ScenarioHacker.CurrentScenarioInfo = value;
        }

        public Scenario Scenario
        {
            get => ScenarioHacker.CurrentScenario ?? throw new InvalidOperationException();
            internal set => ScenarioHacker.CurrentScenario = value;
        }

        public bool IsScenarioCreated => !(ScenarioHacker.CurrentScenario is null);


        [UnderConstruction]
        public void Tick(TimeSpan elapsed)
        {
            double location = Scenario.LocationManager.Location;
            int time = Scenario.TimeManager.TimeMilliseconds;
            double preTrainLocation = Scenario.Route.PreTrainObjects.GetPreTrainLocation(time);
            _ExtendedBeacons?.Tick(location, preTrainLocation);
        }
    }
}
