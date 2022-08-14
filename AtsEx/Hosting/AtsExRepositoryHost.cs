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

namespace Automatic9045.AtsEx.Hosting
{
    internal sealed class AtsExRepositoryHost
    {
        private const string RepositoryOwner = "automatic9045";
        private const string RepositoryName = "AtsEX";

        public AtsExRepositoryHost()
        {
        }

        public Version GetLatestVersion()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("atsex"));
            IReadOnlyList<Release> releases = client.Repository.Release.GetAll(RepositoryOwner, RepositoryName).Result;

            if (releases.Count == 0) throw new Exception("リリースが見つかりません。");

            IEnumerable<Release> stableReleases = releases.Where(r => !r.Prerelease);
            Release latestRelease = stableReleases.Any() ? stableReleases.First() : releases.First();

            string latestVersionText = latestRelease.TagName.TrimStart('v');
            Version latestVersion = Version.Parse(latestVersionText);

            return latestVersion;
        }
    }
}
