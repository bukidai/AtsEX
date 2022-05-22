using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx
{
    internal class AtsExActivator
    {
        public Process TargetProcess { get; }
        public AppDomain TargetAppDomain { get; }
        public Assembly TargetAssembly { get; }

        private Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

        private AssemblyResolver AssemblyResolver;
        private string DirectoryName;

        public AtsExActivator()
        {
            TargetProcess = Process.GetCurrentProcess();
            TargetAppDomain = AppDomain.CurrentDomain;
            TargetAssembly = Assembly.GetEntryAssembly();

            AssemblyResolver = new AssemblyResolver(TargetAppDomain);
            DirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            CheckAssembly();
        }

        public void UpdateIfNotLatest()
        {
            AssemblyResolver.Register(Path.Combine(DirectoryName, "Octokit.dll"));

            try
            {
                AtsExUpdater updater = new AtsExUpdater();
                {
                    bool isRolledBack = updater.RollbackIfRequired(DirectoryName);
                    if (isRolledBack) MessageBox.Show($"前回アップデートの異常終了を検知したため、アップデート前の状態に復元しました。", "AtsEX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                updater.Connect();

                Version currentVersion = ExecutingAssembly.GetName().Version;
                if (currentVersion < updater.LatestVersion)
                {
                    DialogResult confirm = MessageBox.Show($"新しいバージョンの AtsEX がリリースされています。" +
                        $"\n\n現在のバージョン：{currentVersion}\n最新のバージョン：{updater.LatestVersion}\n\n" +
                        $"アップデートしますか？", "AtsEX", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes)
                    {
                        try
                        {
                            updater.Update(DirectoryName);
                            MessageBox.Show($"アップデートが正常に完了しました。", "AtsEX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (NotImplementedException ex)
                        {
                            ShowErrorDialog($"アップデートに失敗しました。\n\n{ex.Message}", "https://automatic9045.github.io/contents/bve/AtsEX/");
                            throw;
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                bool isRolledBack = updater.RollbackIfRequired(DirectoryName);
                                if (isRolledBack)
                                {
                                    ShowErrorDialog($"アップデートに失敗しました。\nAtsEX はアップデート前の状態に復元されました。\n\n\n{ex}");
                                }
                                else
                                {
                                    ShowErrorDialog($"アップデートに失敗しました。\n\n{ex}");
                                }
                            }
                            catch (Exception ex2)
                            {
                                MessageBox.Show($"アップデートに失敗しました。\n\n{ex2}");
                                throw;
                            }
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ResolveAssemblies()
        {
            AssemblyResolver.Register(Path.Combine(DirectoryName, "AtsEx.Core.dll"));
            AssemblyResolver.Register(Path.Combine(DirectoryName, "AtsEx.PluginHost.dll"));
            AssemblyResolver.Register(Path.Combine(DirectoryName, "0Harmony.dll"));
            AssemblyResolver.Register(Path.Combine(DirectoryName, $"Zbx1425.DXDynamicTexture-net{(Environment.Is64BitProcess ? "48" : "35")}.dll"));
            AssemblyResolver.Register(TargetAppDomain.GetAssemblies().First(asm => asm.GetName().Name == "SlimDX"));
        }

        private void CheckAssembly()
        {
            if (TargetAssembly is null)
            {
                ShowErrorDialog("BVE 本体が読み込めないフォーマットです。");
            }
            else if (!TargetAssembly.GetTypes().Any(t => t.Namespace == "Mackoy.Bvets"))
            {
                ShowErrorDialog("BVE 本体と異なるプロセスで実行することはできません。", "https://automatic9045.github.io/contents/bve/AtsEX/faq/#diff-process");
            }
        }

        private void ShowErrorDialog(string message, string faqUrl = null)
        {
            if (faqUrl is null)
            {
                MessageBox.Show(message, $"エラー - AtsEX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show($"{message}\n\nこのエラーに関する情報を表示しますか？\n（ブラウザで Web サイトが開きます）", $"エラー - AtsEX", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start(faqUrl);
                }
            }

            throw new NotSupportedException(message);
        }
    }
}
