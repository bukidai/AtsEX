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

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal sealed class BveHacker : IBveHacker
    {
        public BveHacker(Process targetProcess)
        {
            Process = targetProcess;

            MainFormHacker = new MainFormHacker(Process);
            ScenarioHacker = new ScenarioHacker(MainFormHacker);

            ScenarioHacker.ScenarioCreated += e => PreviewScenarioCreated?.Invoke(e);
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
    }
}
