using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Markdig;

using AtsEx.Properties;

namespace AtsEx.Hosting
{
    internal class AtsExActivator
    {
        public Process TargetProcess { get; }
        public AppDomain TargetAppDomain { get; }
        public Assembly TargetAssembly { get; }

        private readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

        public AtsExActivator()
        {
            TargetProcess = Process.GetCurrentProcess();
            TargetAppDomain = AppDomain.CurrentDomain;
            TargetAssembly = Assembly.GetEntryAssembly();

            CheckAssembly();
        }

        public void CheckUpdates()
        {
            DateTime stoppedUpdateNotificationOn = DateTime.MinValue;
            try
            {
                stoppedUpdateNotificationOn = Settings.Default.StoppedUpdateNotificationOn;
            }
            catch { }

            DateTime now = DateTime.Now;
            if (now < stoppedUpdateNotificationOn)
            {
                Settings.Default.StoppedUpdateNotificationOn = DateTime.MinValue;
                Settings.Default.Save();
            }
            else
            {
                if (now - stoppedUpdateNotificationOn < new TimeSpan(12, 0, 0)) return;
            }

            try
            {
                AtsExRepositoryHost repositoryHost = new AtsExRepositoryHost();

                ReleaseInfo latestRelease = repositoryHost.GetLatestReleaseAsync().Result;
                Version currentVersion = ExecutingAssembly.GetName().Version;

                if (currentVersion < latestRelease.Version || true)
                {
                    UpdateInfoDialog dialog = new UpdateInfoDialog(currentVersion, latestRelease.Version, GetUpdateDetailsHtmlAsync().Result);

                    DialogResult confirm = dialog.ShowDialog();
                    if (confirm == DialogResult.OK)
                    {
                        Process.Start("https://github.com/automatic9045/AtsEX/releases");
                    }

                    if (dialog.DoNotShowAgain)
                    {
                        Settings.Default.StoppedUpdateNotificationOn = now;
                        Settings.Default.Save();
                    }


                    async Task<string> GetUpdateDetailsHtmlAsync()
                        => await Task.Run(() =>
                        {
                            string updateDetailsMarkdown = "【詳細の取得に失敗しました】";
                            try
                            {
                                updateDetailsMarkdown = latestRelease.GetUpdateDetails();
                            }
                            catch { }

                            string updateDetailsHtml = Markdown.ToHtml(updateDetailsMarkdown);
                            return updateDetailsHtml;
                        }).ConfigureAwait(false);
                }
            }
            catch { }
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
