using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HarmonyLib;

using Automatic9045.AtsEx.CoreHackServices;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal sealed class BveHacker : IBveHacker
    {
        public static BveHacker Instance { get; private set; }

        public static void CreateInstance(Process targetProcess)
        {
            Instance = new BveHacker(targetProcess);
        }


        internal ServiceCollection Services { get; }

        private BveHacker(Process targetProcess)
        {
            Process = targetProcess;
            Assembly = App.Instance.BveAssembly;

            Services = CoreHackServiceCollectionBuilder.Build(targetProcess);


            ScenarioHacker.ScenarioProviderCreated += e => ScenarioProviderCreated?.Invoke(e);
        }

        public Process Process { get; }
        public Assembly Assembly { get; }

        public Form MainForm => Services.GetService<IMainFormHacker>().TargetForm;
        public IntPtr MainFormHandle => Services.GetService<IMainFormHacker>().TargetFormHandle;
        public Type MainFormType => Services.GetService<IMainFormHacker>().TargetFormType;


        internal ToolStripItem AddItemToContextMenu(ToolStripItem item) => Services.GetService<IContextMenuHacker>().AddItem(item);
        public ToolStripMenuItem AddClickableMenuItemToContextMenu(string text, EventHandler click) => Services.GetService<IContextMenuHacker>().AddClickableMenuItem(text, click);
        public ToolStripMenuItem AddCheckableMenuItemToContextMenu(string text, EventHandler checkedChanged, bool checkByDefault = false) => Services.GetService<IContextMenuHacker>().AddCheckableMenuItem(text, checkedChanged, checkByDefault);
        public ToolStripMenuItem AddCheckableMenuItemToContextMenu(string text, bool checkByDefault = false) => Services.GetService<IContextMenuHacker>().AddCheckableMenuItem(text, checkByDefault);
        public ToolStripMenuItem AddMenuItemToContextMenu(ToolStripMenuItem item) => Services.GetService<IContextMenuHacker>().AddMenuItem(item);
        internal ToolStripSeparator AddSeparatorToContextMenu() => Services.GetService<IContextMenuHacker>().AddSeparator();


        public Form ScenarioSelectForm => Services.GetService<ISubFormHacker>().ScenarioSelectForm;
        public Form LoadingProgressForm => Services.GetService<ISubFormHacker>().LoadingProgressForm;

        public Form TimePosForm => Services.GetService<ISubFormHacker>().TimePosForm;
        public Form ChartForm => Services.GetService<ISubFormHacker>().ChartForm;


        public void ThrowError(string text, string senderFileName = "", int lineIndex = 0, int charIndex = 0) => Services.GetService<ILoadErrorHacker>().ThrowError(text, senderFileName, lineIndex, charIndex);
        public void ThrowError(LoadError error) => Services.GetService<ILoadErrorHacker>().ThrowError(error);
        public void ThrowError(IEnumerable<LoadError> errors) => Services.GetService<ILoadErrorHacker>().ThrowErrors(errors);


        public event ScenarioProviderCreatedEventHandler ScenarioProviderCreated;

        public ScenarioInfo CurrentScenarioInfo
        {
            get => Services.GetService<IScenarioHacker>().CurrentScenarioInfo;
            set => Services.GetService<IScenarioHacker>().CurrentScenarioInfo = value as ScenarioInfo;
        }

        public ScenarioProvider CurrentScenarioProvider
        {
            get => Services.GetService<IScenarioHacker>().CurrentScenarioProvider ?? throw new InvalidOperationException();
            internal set => Services.GetService<IScenarioHacker>().CurrentScenarioProvider = value as ScenarioProvider;
        }

        public bool HasScenarioProviderCreated
        {
            get => !(Services.GetService<IScenarioHacker>().CurrentScenarioProvider is null);
        }


        public void UpdateDiagram() => Services.GetService<IDiagramHacker>().Update();


        [WillRefactor]
        public VehicleSpec VehicleSpec { get; internal set; } = null;

        [WillRefactor]
        public VehicleState VehicleState { get; internal set; } = null;


        public void Dispose()
        {
            Services.Dispose();
        }
    }
}
