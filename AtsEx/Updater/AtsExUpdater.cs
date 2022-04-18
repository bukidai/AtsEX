using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Octokit;

namespace Automatic9045.AtsEx.Export
{
    internal sealed class AtsExUpdater
    {
        private Release LatestRelease = null;

        private Version latestVersion = null;
        public Version LatestVersion
        {
            get
            {
                if (latestVersion is null) throw new InvalidOperationException($"{nameof(Connect)} メソッドが実行されていません。");
                return latestVersion;
            }
            private set => latestVersion = value;
        }

        public AtsExUpdater()
        {
        }

        public bool RollbackIfRequired(string targetDirectoryName)
        {
            string backupDirectoryName = Path.Combine(targetDirectoryName, ".AtsExBackup");
            if (!Directory.Exists(backupDirectoryName)) return false;

            string tempFilePath = Path.Combine(backupDirectoryName, ".Temp");
            if (!File.Exists(tempFilePath)) return false;

            string[] backedupFilePaths = Directory.GetFiles(backupDirectoryName);
            foreach (string backedupFilePath in backedupFilePaths)
            {
                if (backedupFilePath == tempFilePath) continue;

                string destPath = Path.Combine(targetDirectoryName, Path.GetFileName(backedupFilePath));
                if (File.Exists(destPath)) File.Delete(destPath);
                File.Move(backedupFilePath, destPath);
            }

            File.Delete(Path.Combine(backupDirectoryName, ".Temp"));
            return true;
        }

        public void Connect()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("atsex"));
            IReadOnlyList<Release> releases = client.Repository.Release.GetAll("automatic9045", "AtsEX").Result;

            if (releases.Count == 0) throw new Exception("リリースが見つかりません。");

            IEnumerable<Release> stableReleases = releases.Where(r => !r.Prerelease);
            if (stableReleases.Any())
            {
                LatestRelease = stableReleases.First();
            }
            else
            {
                LatestRelease = releases.First();
            }

            string latestVersionText = LatestRelease.TagName.TrimStart('v');
            LatestVersion = Version.Parse(latestVersionText);
        }

        public void Update(string targetDirectoryName)
        {
            ReleaseAsset packageAsset = LatestRelease.Assets.FirstOrDefault(a => a.Name == "AutoUpdate.atsexpkg");

            if (packageAsset is null) throw new Exception("AtsEX 自動更新用パッケージが見つかりません。");

            ZipArchive package;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(packageAsset.BrowserDownloadUrl).Result;
                if (!response.IsSuccessStatusCode) throw new Exception("AtsEX 自動更新用パッケージのダウンロードに失敗しました。");

                Stream packageStream = response.Content.ReadAsStreamAsync().Result;
                package = new ZipArchive(packageStream);
            }

            string backupDirectoryName = Path.Combine(targetDirectoryName, ".AtsExBackup");
            if (Directory.Exists(backupDirectoryName))
            {
                Directory.Delete(backupDirectoryName, true);
            }

            ZipArchiveEntry packageConfigEntry = package.Entries.FirstOrDefault(entry => entry.Name == "Package.xml");
            AutoUpdatePackageInfo packageInfo = AutoUpdatePackageInfo.Default;
            if (!(packageConfigEntry is null))
            {
                using (Stream packageConfigStream = packageConfigEntry.Open())
                {
                    XDocument packageConfig = XDocument.Load(packageConfigStream);
                    packageInfo = AutoUpdatePackageInfo.FromXDocument(packageConfig);
                }
            }

            if (packageInfo.IncludesMainAssembly)
            {
                throw new NotImplementedException(
                    $"このバージョンの AtsEX 自動更新用パッケージ (バージョン {LatestVersion}) は自動で適用できません。\n" +
                    $"恐れ入りますが、ホームページから手動でのダウンロードをお願いします。");
            }

            Directory.CreateDirectory(backupDirectoryName);

            string tempFilePath = Path.Combine(backupDirectoryName, ".Temp");
            File.Create(tempFilePath).Close();
            File.SetAttributes(tempFilePath, FileAttributes.Hidden);

            foreach (ZipArchiveEntry entry in package.Entries)
            {
                if (entry == packageConfigEntry) continue;

                string filePath = Path.Combine(targetDirectoryName, entry.FullName);
                if (File.Exists(filePath))
                {
                    File.Move(filePath, Path.Combine(backupDirectoryName, entry.FullName));
                }

                entry.ExtractToFile(filePath);
            }

            File.Delete(Path.Combine(backupDirectoryName, ".Temp"));
        }
    }
}
