using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    internal class ProfileSelector
    {
        protected static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<ProfileSelector>("PluginHost");

        private static readonly string DefaultNamespace = $"{typeof(BveTypeSet).Namespace}.TypeNameDefinitions";

        private readonly Assembly ExecutingAssembly;
        private readonly Version BveVersion;

        public ProfileSelector(Version bveVersion)
        {
            ExecutingAssembly = Assembly.GetExecutingAssembly();
            BveVersion = bveVersion;
        }

        public Profile GetProfileStream(bool allowNotSupportedVersion)
        {
            Version version = GetVersion(allowNotSupportedVersion);
            Stream stream = ExecutingAssembly.GetManifestResourceStream($"{DefaultNamespace}.{version}.xml");
            return new Profile(stream, version);
        }

        private Version GetVersion(bool allowNotSupportedVersion)
        {
            string[] resourceNames = ExecutingAssembly.GetManifestResourceNames();
            if (resourceNames.Contains($"{DefaultNamespace}.{BveVersion}.xml"))
            {
                return BveVersion;
            }
            else
            {
                if (allowNotSupportedVersion)
                {
                    List<Version> supportedAllVersions = new List<Version>();
                    foreach (string name in resourceNames)
                    {
                        if (!name.StartsWith($"{DefaultNamespace}.") || !name.EndsWith(".xml")) continue;

                        string versionText = name.Substring(DefaultNamespace.Length + 1, name.Length - (DefaultNamespace.Length + 1 + ".xml".Length));
                        Version version;
                        if (Version.TryParse(versionText, out version))
                        {
                            supportedAllVersions.Add(version);
                        }
                    }

                    List<Version> supportedVersions = supportedAllVersions.FindAll(version => version.Major == BveVersion.Major);

                    if (supportedVersions.Any())
                    {
                        Version latestVersion = supportedVersions.Max();
                        Version oldestVersion = supportedVersions.Min();
                        if (BveVersion < latestVersion)
                        {
                            if (BveVersion < oldestVersion)
                            {
                                throw new NotSupportedException(string.Format(Resources.GetString("VersionTooOld").Value, BveVersion, oldestVersion));
                            }
                            else
                            {
                                throw new NotSupportedException(string.Format(Resources.GetString("InvalidVersion").Value, BveVersion));
                            }
                        }

                        return latestVersion;
                    }
                    else
                    {
                        IEnumerable<Version> newerVersions = supportedAllVersions.Where(version => version.Major < BveVersion.Major);
                        if (newerVersions.Any())
                        {
                            return newerVersions.Max();
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format(Resources.GetString("MajorVersionTooOld").Value, BveVersion, supportedAllVersions.Min().Major));
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException(string.Format(Resources.GetString("VersionNotSupported").Value, BveVersion));
                }
            }
        }

        public Stream GetSchemaStream()
        {
            Stream stream = ExecutingAssembly.GetManifestResourceStream($"{DefaultNamespace}.BveTypesXmlSchema.xsd");
            return stream;
        }
    }
}
