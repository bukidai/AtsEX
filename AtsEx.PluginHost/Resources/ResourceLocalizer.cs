using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Resources
{
    public partial class ResourceLocalizer
    {
        protected static readonly Func<CultureInfo> DefaultGetCurrentCultureFunc = () => CultureInfo.CurrentUICulture;

        protected readonly SortedList<string, SortedList<CultureInfo, object>> Resources;
        protected readonly Func<CultureInfo> GetCurrentCultureFunc;

        public static ResourceLocalizer FromResX(string neutralResourcePath, Func<CultureInfo> getCurrentCultureFunc)
        {
            if (Path.GetExtension(neutralResourcePath) != ".resx") throw new ArgumentException($"ファイル '{neutralResourcePath}' の拡張子が .resx ではありません。");

            ResourceSet neutralResource = ResourceSet.FromResX(neutralResourcePath);

            Dictionary<string, SortedList<CultureInfo, object>> _allResources = neutralResource.
                ToDictionary(x => x.Key, x => new SortedList<CultureInfo, object>(new CultureComparer()) { { CultureInfo.InvariantCulture, x.Value } });
            SortedList<string, SortedList<CultureInfo, object>> allResources = new SortedList<string, SortedList<CultureInfo, object>>(_allResources);

            string directoryName = Path.GetDirectoryName(neutralResourcePath);
            string neutralFileName = Path.GetFileNameWithoutExtension(neutralResourcePath);

            string[] resourceFilePaths = Directory.GetFiles(directoryName, $"{neutralFileName}.*.resx");
            foreach (string resourceFilePath in resourceFilePaths)
            {
                CultureInfo culture = GetCultureFromResourcePath(resourceFilePath);
                ResourceSet resources = ResourceSet.FromResX(resourceFilePath);

                foreach (KeyValuePair<string, object> item in resources)
                {
                    allResources[item.Key][culture] = item.Value;
                }
            }

            return new ResourceLocalizer(allResources, getCurrentCultureFunc);


            CultureInfo GetCultureFromResourcePath(string resourcePath)
            {
                string fileName = Path.GetFileNameWithoutExtension(resourcePath);
                string cultureName = fileName.Substring(neutralFileName.Length + 1);

                CultureInfo culture = new CultureInfo(cultureName);
                return culture;
            }
        }

        public static ResourceLocalizer FromResX(string neutralResourcePath) => FromResX(neutralResourcePath, DefaultGetCurrentCultureFunc);

        public static ResourceLocalizer FromResXOfType(Type targetType, string subDirectory, Func<CultureInfo> getCurrentCultureFunc)
            => FromResX(Path.Combine(Path.GetDirectoryName(targetType.Assembly.Location), "Resources", subDirectory, $"{targetType.Name}.resx"), getCurrentCultureFunc);

        public static ResourceLocalizer FromResXOfType(Type targetType, string subDirectory) => FromResXOfType(targetType, subDirectory, DefaultGetCurrentCultureFunc);

        public static ResourceLocalizer FromResXOfType<TTarget>(string subDirectory, Func<CultureInfo> getCurrentCultureFunc)
            => FromResXOfType(typeof(TTarget), subDirectory, getCurrentCultureFunc);

        public static ResourceLocalizer FromResXOfType<TTarget>(string subDirectory) => FromResXOfType(typeof(TTarget), subDirectory);


        protected ResourceLocalizer(SortedList<string, SortedList<CultureInfo, object>> resources, Func<CultureInfo> getCurrentCultureFunc)
        {
            Resources = resources;
            GetCurrentCultureFunc = getCurrentCultureFunc;
        }

        protected ResourceLocalizer(SortedList<string, SortedList<CultureInfo, object>> resources) : this(resources, DefaultGetCurrentCultureFunc)
        {
        }


        public ResourceInfo<T> Get<T>(string key)
        {
            SortedList<CultureInfo, object> valueList = Resources[key];
            CultureInfo culture = ResolveCulture(GetCurrentCultureFunc(), valueList.Keys);

            return new ResourceInfo<T>(culture, (T)valueList[culture]);
        }

        public ResourceInfo<string> GetString(string key) => Get<string>(key);

        public ResourceInfo<int> GetInt32(string key) => Get<int>(key);

        protected static CultureInfo ResolveCulture(CultureInfo culture, IEnumerable<CultureInfo> candidates)
        {
            return candidates.Contains(culture) ? culture
                : culture == CultureInfo.InvariantCulture ? throw new InvalidOperationException($"{nameof(culture)} およびその親カルチャが {nameof(candidates)} に含まれていません。")
                : ResolveCulture(culture.Parent, candidates);
        }
    }
}
