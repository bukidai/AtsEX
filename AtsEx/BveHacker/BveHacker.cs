using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.CoreHackServices;
using Automatic9045.AtsEx.ClassWrappers;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal sealed class BveHacker : IBveHacker
    {
        private App App;

        internal ServiceCollection Services { get; }

        internal BveHacker(App app, Process targetProcess, Assembly targetAssembly)
        {
            App = app;

            Process = targetProcess;
            Assembly = targetAssembly;

            Services = CoreHackServiceCollectionBuilder.Build(targetProcess, targetAssembly);
        }

        [UnderConstruction]
        public void ThrowError(string text, string senderFileName = null, int lineIndex = 0, int charIndex = 0)
        {
            MessageBox.Show($"{text}\n\n場所：{senderFileName}\n行：{lineIndex}\n列：{charIndex}", $"エラー - {App.ProductShortName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        [UnderConstruction]
        public void ThrowWarning(string text, string senderFileName = null, int lineIndex = 0, int charIndex = 0)
        {
            MessageBox.Show($"{text}\n\n場所：{senderFileName}\n行：{lineIndex}\n列：{charIndex}", $"エラー - {App.ProductShortName}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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


        public IScenarioInfo CurrentScenarioInfo
        {
            get => Services.GetService<IScenarioHacker>().CurrentScenarioInfo;
            set => Services.GetService<IScenarioHacker>().CurrentScenarioInfo = value as ScenarioInfo;
        }

        public IScenarioProvider CurrentScenarioProvider
        {
            get => Services.GetService<IScenarioHacker>().CurrentScenarioProvider;
            internal set => Services.GetService<IScenarioHacker>().CurrentScenarioProvider = value as ScenarioProvider;
        }

        public void Dispose()
        {
            Services.Dispose();
        }
    }
}
