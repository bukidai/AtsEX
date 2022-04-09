using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost
{
    public delegate void ScenarioProviderCreatedEventHandler(IScenarioProvider scenarioProvider);

    public interface IBveHacker : IDisposable
    {
        /// <summary>
        /// 制御対象の BVE を実行している <see cref="System.Diagnostics.Process"/> を取得します。
        /// </summary>
        Process Process { get; }

        /// <summary>
        /// 制御対象の BVE の <see cref="Assembly"/> を取得します。
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// BVE のメインフォームを取得します。
        /// </summary>
        Form MainForm { get; }

        /// <summary>
        /// BVE のメインフォームのハンドルを取得します。
        /// </summary>
        IntPtr MainFormHandle { get; }

        /// <summary>
        /// BVE のメインフォームの <see cref="Type"/> を取得します。<see cref="MainForm.GetType()"/> と同じ実行結果が得られます。
        /// </summary>
        Type MainFormType { get; }


        /// <summary>
        /// BVE の右クリックメニューにクリック可能な <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text">メニューに表示するテキスト。</param>
        /// <param name="click">クリックされると発生する <see cref="ToolStripItem.Click"/> イベントに登録する <see cref="EventHandler"/>。</param>
        ToolStripMenuItem AddClickableMenuItemToContextMenu(string text, EventHandler click);

        /// <summary>
        /// BVE の右クリックメニューにチェック可能な <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text">メニューに表示するテキスト。</param>
        /// <param name="checkedChanged">チェック・アンチェックされると発生する <see cref="ToolStripMenuItem.CheckedChanged"/> イベントに登録する <see cref="EventHandler"/>。</param>
        /// <param name="checkByDefault">デフォルトでチェックするか。</param>
        ToolStripMenuItem AddCheckableMenuItemToContextMenu(string text, EventHandler checkedChanged, bool checkByDefault = false);

        /// <summary>
        /// BVE の右クリックメニューにチェック可能な <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text">メニューに表示するテキスト。</param>
        /// <param name="checkByDefault">デフォルトでチェックするか。</param>
        ToolStripMenuItem AddCheckableMenuItemToContextMenu(string text, bool checkByDefault = false);

        /// <summary>
        /// BVE の右クリックメニューに独自の <see cref="ToolStripMenuItem"/> を追加します。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="click"></param>
        ToolStripMenuItem AddMenuItemToContextMenu(ToolStripMenuItem item);


        /// <summary>
        /// BVE の「シナリオの選択」フォームを取得します。
        /// </summary>
        Form ScenarioSelectForm { get; }

        /// <summary>
        /// BVE の「シナリオを読み込んでいます...」フォームを取得します。
        /// </summary>
        Form LoadingProgressForm { get; }

        /// <summary>
        /// BVE の「時刻と位置」フォームを取得します。
        /// </summary>
        Form TimePosForm { get; }

        /// <summary>
        /// BVE の「車両物理量」フォームを取得します。
        /// </summary>
        Form ChartForm { get; }


        void ThrowError(string text, string senderFileName = "", int lineIndex = 0, int charIndex = 0);
        void ThrowError(ILoadError error);
        void ThrowError(IEnumerable<ILoadError> errors);


        /// <summary>
        /// <see cref="IScenarioProvider"/> のインスタンスが生成されたときに通知します。
        /// </summary>
        event ScenarioProviderCreatedEventHandler ScenarioProviderCreated;

        /// <summary>
        /// 現在読込中または実行中のシナリオの情報を取得・設定します。
        /// </summary>
        IScenarioInfo CurrentScenarioInfo { get; set; }

        /// <summary>
        /// 現在実行中のシナリオを取得します。シナリオの読込中は <see cref="InvalidOperationException"/> をスローします。
        /// シナリオの読込中に <see cref="IScenarioProvider"/> を取得するには <see cref="ScenarioProviderCreated"/> イベントを購読してください。
        /// </summary>
        IScenarioProvider CurrentScenarioProvider { get; }


        /// <summary>
        /// BVE の「時刻と位置」フォームのダイヤグラムを再描画します。
        /// </summary>
        void UpdateDiagram();
    }
}
