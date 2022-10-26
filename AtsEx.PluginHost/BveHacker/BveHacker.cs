using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Mackoy.Bvets;

using UnembeddedResources;

using AtsEx.PluginHost.BveHackerServices;
using AtsEx.PluginHost.BveTypes;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.ExtendedBeacons;

namespace AtsEx.PluginHost
{
    /// <summary>
    /// <see cref="BveHacker.ScenarioCreated"/> または <see cref="BveHacker.PreviewScenarioCreated"/> イベントを処理するメソッドを表します。
    /// </summary>
    /// <param name="e">イベント データを格納している <see cref="ScenarioCreatedEventArgs"/>。</param>
    public delegate void ScenarioCreatedEventHandler(ScenarioCreatedEventArgs e);

    public abstract class BveHacker : IDisposable
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<BveHacker>("PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> IllegalSlimDXDetected { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> CannotGetScenario { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static BveHacker()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private readonly StructureSetLifeProlonger StructureSetLifeProlonger;

        protected BveHacker(Action<Version> profileForDifferentBveVersionLoaded)
        {
            try
            {
                BveTypes = BveTypeSet.LoadAsync(App.Instance.BveAssembly, App.Instance.BveVersion, true, profileForDifferentBveVersionLoaded).Result;
            }
            catch (KeyNotFoundException)
            {
                CheckSlimDX();
                throw;
            }

            ClassWrapperInitializer classWrapperInitializer = new ClassWrapperInitializer(this);
            classWrapperInitializer.InitializeAllAsync().Wait();

            BveTypesSetter bveTypesSetter = new BveTypesSetter(this);
            bveTypesSetter.InitializeAllAsync().Wait();

            MainFormHacker = new MainFormHacker(App.Instance.Process);
            ScenarioHacker = new ScenarioHacker(MainFormHacker, BveTypes);

            StructureSetLifeProlonger = new StructureSetLifeProlonger(this);

            LoadErrorManager = new LoadErrorManager(LoadingProgressForm);


            ScenarioHacker.ScenarioCreated += e => PreviewScenarioCreated?.Invoke(e);
            ScenarioHacker.ScenarioCreated += OnScenarioCreated;
            ScenarioHacker.ScenarioCreated += e => ScenarioCreated?.Invoke(e);
        }

        private void CheckSlimDX()
        {
            IEnumerable<Assembly> slimDXAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(asm => asm.GetName().Name == "SlimDX");

            if (slimDXAssemblies.Count() > 1)
            {
                string locationText = string.Join("\n", slimDXAssemblies.Select(assembly => "・" + assembly.Location));
                MessageBox.Show(string.Format(Resources.Value.IllegalSlimDXDetected.Value, locationText), App.Instance.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// <see cref="PreviewScenarioCreated"/> が発生した後、<see cref="ScenarioCreated"/> が発生する前に呼び出されます。
        /// </summary>
        /// <param name="e">イベント データを格納している <see cref="ScenarioCreatedEventArgs"/>。</param>
        protected abstract void OnScenarioCreated(ScenarioCreatedEventArgs e);

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            StructureSetLifeProlonger.Dispose();
            ScenarioHacker.Dispose();
        }

        /// <summary>
        /// クラスラッパーに対応する BVE の型とメンバーの定義を表す <see cref="BveTypeSet"/> を取得します。
        /// </summary>
        public BveTypeSet BveTypes { get; }


        private readonly MainFormHacker MainFormHacker;

        /// <summary>
        /// BVE のメインフォームのハンドルを取得します。
        /// </summary>
        public IntPtr MainFormHandle => MainFormHacker.TargetFormHandle;

        /// <summary>
        /// BVE のメインフォームの <see cref="Form"/> インスタンスを取得します。
        /// </summary>
        public Form MainFormSource => MainFormHacker.TargetFormSource;

        /// <summary>
        /// BVE のメインフォームを取得します。
        /// </summary>
        public MainForm MainForm => MainFormHacker.TargetForm;


        /// <summary>
        /// BVE の「シナリオの選択」フォームの <see cref="Form"/> インスタンスを取得します。
        /// </summary>
        public Form ScenarioSelectionFormSource => ScenarioSelectionForm.Src;

        /// <summary>
        /// BVE の「シナリオの選択」フォームを取得します。
        /// </summary>
        public ScenarioSelectionForm ScenarioSelectionForm => MainForm.ScenarioSelectForm;

        /// <summary>
        /// BVE の「シナリオを読み込んでいます...」フォームの <see cref="Form"/> インスタンスを取得します。
        /// </summary>
        public Form LoadingProgressFormSource => LoadingProgressForm.Src;

        /// <summary>
        /// BVE の「シナリオを読み込んでいます...」フォームを取得します。
        /// </summary>
        public LoadingProgressForm LoadingProgressForm => MainForm.LoadingProgressForm;

        /// <summary>
        /// BVE の「時刻と位置」フォームの <see cref="Form"/> インスタンスを取得します。
        /// </summary>
        public Form TimePosFormSource => TimePosForm.Src;

        /// <summary>
        /// BVE の「時刻と位置」フォームを取得します。
        /// </summary>
        public TimePosForm TimePosForm => MainForm.TimePosForm;

        /// <summary>
        /// BVE の「車両物理量」フォームの <see cref="Form"/> インスタンスを取得します。
        /// </summary>
        public Form ChartFormSource => ChartForm.Src;

        /// <summary>
        /// BVE の「車両物理量」フォームを取得します。
        /// </summary>
        public ChartForm ChartForm => MainForm.ChartForm;


        /// <summary>
        /// BVE の設定が格納された <see cref="Mackoy.Bvets.Preferences"/> を取得します。
        /// </summary>
        public Preferences Preferences => MainForm.Preferences;

        /// <summary>
        /// キー入力を管理する <see cref="ClassWrappers.KeyProvider"/> を取得します。
        /// </summary>
        public KeyProvider KeyProvider => MainForm.KeyProvider;


        /// <summary>
        /// シナリオ読込時のエラーを編集するための機能を提供する <see cref="PluginHost.LoadErrorManager"/> を取得します。
        /// </summary>
        public LoadErrorManager LoadErrorManager { get; }


        /// <summary>
        /// メインフォームの右クリックメニューを編集するための機能を提供する <see cref="IContextMenuHacker"/> を取得します。
        /// </summary>
        public abstract IContextMenuHacker ContextMenuHacker { get; }


        /// <summary>
        /// 全てのハンドルのセットを取得します。
        /// </summary>
        /// <remarks>
        /// <see cref="BveHacker"/> が利用できない場合は <see cref="IApp.Handles"/> プロパティを使用してください。
        /// ただし、<see cref="IApp.Handles"/> プロパティに設定されている値は力行ハンドルの抑速ノッチ、ブレーキハンドルの抑速ブレーキノッチを無視したものになります。
        /// </remarks>
        /// <seealso cref="IApp.Handles"/>
        public abstract Handles.HandleSet Handles { get; }

        /// <summary>
        /// 拡張地上子の一覧を取得します。
        /// </summary>
        public abstract ExtendedBeaconSet ExtendedBeacons { get; }


        private readonly ScenarioHacker ScenarioHacker;

        /// <summary>
        /// <see cref="ScenarioCreated"/> が発生する直前に通知します。特に理由がなければ <see cref="ScenarioCreated"/> を使用してください。
        /// </summary>
        public event ScenarioCreatedEventHandler PreviewScenarioCreated;

        /// <summary>
        /// <see cref="ClassWrappers.Scenario"/> のインスタンスが生成されたときに通知します。
        /// </summary>
        public event ScenarioCreatedEventHandler ScenarioCreated;

        /// <summary>
        /// 現在読込中または実行中のシナリオの情報を取得・設定します。
        /// </summary>
        public ScenarioInfo ScenarioInfo
        {
            get => ScenarioHacker.CurrentScenarioInfo;
            set => ScenarioHacker.CurrentScenarioInfo = value;
        }

        /// <summary>
        /// 現在実行中のシナリオを取得します。シナリオの読込中は <see cref="InvalidOperationException"/> をスローします。
        /// シナリオの読込中に <see cref="ClassWrappers.Scenario"/> を取得するには <see cref="ScenarioCreated"/> イベントを購読してください。
        /// </summary>
        public Scenario Scenario => ScenarioHacker.CurrentScenario ?? throw new InvalidOperationException(string.Format(Resources.Value.CannotGetScenario.Value, nameof(Scenario)));

        /// <summary>
        /// <see cref="Scenario"/> が取得可能かどうかを取得します。
        /// </summary>
        public bool IsScenarioCreated => !(ScenarioHacker.CurrentScenario is null);
    }
}
