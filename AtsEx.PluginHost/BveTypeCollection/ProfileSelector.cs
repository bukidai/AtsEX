using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    internal class ProfileSelector
    {
        private static readonly string DefaultNamespace = $"{typeof(BveTypeCollectionProvider).Namespace}.TypeNameDefinitions";

        private Assembly ExecutingAssembly;
        private Version BveVersion;

        public ProfileSelector(Assembly bveAssembly)
        {
            ExecutingAssembly = Assembly.GetExecutingAssembly();
            BveVersion = bveAssembly.GetName().Version;
        }

        public ProfileInfo GetProfileStream(bool allowNotSupportedVersion)
        {
            Version version = GetVersion(allowNotSupportedVersion);
            Stream stream = ExecutingAssembly.GetManifestResourceStream($"{DefaultNamespace}.{version}.xml");
            return new ProfileInfo(stream, version);
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
                    IEnumerable<Version> supportedAllVersions = resourceNames.Select(name => Version.Parse(name.Replace(".xml", "")));
                    IEnumerable<Version> supportedVersions = supportedAllVersions.Where(version => version.Major == BveVersion.Major);
                    if (supportedVersions.Any())
                    {
                        Version latestVersion = supportedVersions.Max();
                        Version oldestVersion = supportedVersions.Min();
                        if (BveVersion < latestVersion)
                        {
                            if (BveVersion < oldestVersion)
                            {
                                throw new NotSupportedException($"BVE バージョン {BveVersion} には対応していません。{oldestVersion} 以降のみサポートしています。");
                            }
                            else
                            {
                                throw new NotSupportedException($"BVE バージョン {BveVersion} は認識されていません。サポートされないバージョンであるか、不正なバージョンです。");
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
                            throw new NotSupportedException($"BVE バージョン {BveVersion} には対応していません。{supportedAllVersions.Min().Major}.x 以降のみサポートしています。");
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException($"BVE バージョン {BveVersion} には対応していません。");
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
