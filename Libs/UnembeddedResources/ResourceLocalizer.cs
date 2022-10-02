using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace UnembeddedResources
{
    /// <summary>
    /// 外部のリソースファイルを利用して複数の言語に対応するための機能を提供します。
    /// </summary>
    /// <remarks>
    /// アセンブリに埋め込まれたリソースを利用した多言語対応の方法は .NET が標準で提供していますが、
    /// このクラスでは、アセンブリに埋め込まれていないそのままのリソース (.resx) ファイルを参照できます。<br/>
    /// リソースファイルをアセンブリに埋め込まないことで、プラグインの作者以外の方も容易に言語パックを開発できるようになります。
    /// </remarks>
    public partial class ResourceLocalizer
    {
        /// <summary>
        /// <see cref="GetCurrentCultureFunc"/> の既定値として使用されるデリゲートを取得します。
        /// </summary>
        protected static readonly Func<CultureInfo> DefaultGetCurrentCultureFunc = () => CultureInfo.CurrentUICulture;

        /// <summary>
        /// リソースの一覧を取得します。
        /// </summary>
        internal protected Dictionary<string, Dictionary<CultureInfo, object>> Resources { get; }

        /// <summary>
        /// 現在のカルチャを取得するデリケートを取得します。
        /// </summary>
        protected readonly Func<CultureInfo> GetCurrentCultureFunc;

        /// <summary>
        /// リソース (.resx) ファイルから新しい <see cref="ResourceLocalizer"/> を作成します。
        /// </summary>
        /// <remarks>
        /// <paramref name="neutralResourcePath"/> へ指定した URI を基に、各カルチャ向けのリソースファイルの URI を推測して読み込みます。<br/>
        /// 例えば、<c>A\B.resx</c> を指定した場合、<c>A\B.ja.resx</c> がカルチャ ja 向け、<c>A\B.ja-JP.resx</c> がカルチャ ja-JP 向けのリソースとして認識されます。
        /// </remarks>
        /// <param name="neutralResourcePath">ニュートラル カルチャのリソースファイルを参照している URI 文字列。</param>
        /// <param name="getCurrentCultureFunc">現在のカルチャを取得するデリケート。</param>
        /// <returns>読み込んだリソースを格納している <see cref="ResourceLocalizer"/>。</returns>
        public static ResourceLocalizer FromResX(string neutralResourcePath, Func<CultureInfo> getCurrentCultureFunc)
        {
            if (Path.GetExtension(neutralResourcePath) != ".resx") throw new ArgumentException($"ファイル '{neutralResourcePath}' の拡張子が .resx ではありません。");

            ResourceSet neutralResource = ResourceSet.FromResX(neutralResourcePath);

            Dictionary<string, Dictionary<CultureInfo, object>> allResources = neutralResource.
                ToDictionary(x => x.Key, x => new Dictionary<CultureInfo, object>() { { CultureInfo.InvariantCulture, x.Value } });

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

        /// <summary>
        /// リソース (.resx) ファイルから新しい <see cref="ResourceLocalizer"/> を作成します。
        /// </summary>
        /// <remarks>
        /// <paramref name="neutralResourcePath"/> へ指定した URI を基に、各カルチャ向けのリソースファイルの URI を推測して読み込みます
        /// (例: <c>A\B\SampleClass.resx</c> を指定した場合、<c>A\V\SampleClass.ja.resx</c> がカルチャ ja 向け、<c>A\B\SampleClass.ja-JP.resx</c> がカルチャ ja-JP 向けのリソースとして認識されます)。
        /// </remarks>
        /// <param name="neutralResourcePath">ニュートラル カルチャのリソースファイルを参照している URI 文字列。</param>
        /// <returns>読み込んだリソースを格納している <see cref="ResourceLocalizer"/>。</returns>
        public static ResourceLocalizer FromResX(string neutralResourcePath) => FromResX(neutralResourcePath, DefaultGetCurrentCultureFunc);

        /// <summary>
        /// リソース (.resx) ファイルから新しい <see cref="ResourceLocalizer"/> を作成します。
        /// </summary>
        /// <remarks>
        /// <paramref name="targetType"/>、<paramref name="subDirectory"/> へ指定した値を基に、各カルチャ向けのリソースファイルの URI を推測して読み込みます
        /// (例: <paramref name="targetType"/> に <c>SampleClass</c>、<paramref name="subDirectory"/> に <c>A\B</c> を指定した場合、
        /// <c>A\B\SampleClass.resx</c> がニュートラル カルチャのリソース、<c>A\B\SampleClass.ja-JP.resx</c> がカルチャ ja-JP 向けのリソースとして認識されます)。
        /// </remarks>
        /// <param name="targetType">対象とする型。</param>
        /// <param name="subDirectory"><paramref name="targetType"/> が定義されたアセンブリを格納しているディレクトリから、リソースファイルを格納しているディレクトリへの相対パス。</param>
        /// <param name="getCurrentCultureFunc">現在のカルチャを取得するデリケート。</param>
        /// <returns>読み込んだリソースを格納している <see cref="ResourceLocalizer"/>。</returns>
        public static ResourceLocalizer FromResXOfType(Type targetType, string subDirectory, Func<CultureInfo> getCurrentCultureFunc)
            => FromResX(Path.Combine(Path.GetDirectoryName(targetType.Assembly.Location), "Resources", subDirectory, $"{targetType.Name}.resx"), getCurrentCultureFunc);

        /// <summary>
        /// リソース (.resx) ファイルから新しい <see cref="ResourceLocalizer"/> を作成します。
        /// </summary>
        /// <remarks>
        /// <paramref name="targetType"/>、<paramref name="subDirectory"/> へ指定した値を基に、各カルチャ向けのリソースファイルの URI を推測して読み込みます
        /// (例: <paramref name="targetType"/> に <c>SampleClass</c>、<paramref name="subDirectory"/> に <c>A\B</c> を指定した場合、
        /// <c>A\B\SampleClass.resx</c> がニュートラル カルチャのリソース、<c>A\B\SampleClass.ja-JP.resx</c> がカルチャ ja-JP 向けのリソースとして認識されます)。
        /// </remarks>
        /// <param name="targetType">対象とする型。</param>
        /// <param name="subDirectory"><paramref name="targetType"/> が定義されたアセンブリを格納しているディレクトリから、リソースファイルを格納しているディレクトリへの相対パス。</param>
        /// <returns>読み込んだリソースを格納している <see cref="ResourceLocalizer"/>。</returns>
        public static ResourceLocalizer FromResXOfType(Type targetType, string subDirectory) => FromResXOfType(targetType, subDirectory, DefaultGetCurrentCultureFunc);

        /// <summary>
        /// リソース (.resx) ファイルから新しい <see cref="ResourceLocalizer"/> を作成します。
        /// </summary>
        /// <remarks>
        /// <typeparamref name="TTarget"/>、<paramref name="subDirectory"/> へ指定した値を基に、各カルチャ向けのリソースファイルの URI を推測して読み込みます
        /// (例: <typeparamref name="TTarget"/> に <c>SampleClass</c>、<paramref name="subDirectory"/> に <c>A\B</c> を指定した場合、
        /// <c>A\B\SampleClass.resx</c> がニュートラル カルチャのリソース、<c>A\B\SampleClass.ja-JP.resx</c> がカルチャ ja-JP 向けのリソースとして認識されます)。
        /// </remarks>
        /// <typeparam name="TTarget">対象とする型。</typeparam>
        /// <param name="subDirectory"><typeparamref name="TTarget"/> が定義されたアセンブリを格納しているディレクトリから、リソースファイルを格納しているディレクトリへの相対パス。</param>
        /// <param name="getCurrentCultureFunc">現在のカルチャを取得するデリケート。</param>
        /// <returns>読み込んだリソースを格納している <see cref="ResourceLocalizer"/>。</returns>
        public static ResourceLocalizer FromResXOfType<TTarget>(string subDirectory, Func<CultureInfo> getCurrentCultureFunc)
            => FromResXOfType(typeof(TTarget), subDirectory, getCurrentCultureFunc);

        /// <summary>
        /// リソース (.resx) ファイルから新しい <see cref="ResourceLocalizer"/> を作成します。
        /// </summary>
        /// <remarks>
        /// <typeparamref name="TTarget"/>、<paramref name="subDirectory"/> へ指定した値を基に、各カルチャ向けのリソースファイルの URI を推測して読み込みます
        /// (例: <typeparamref name="TTarget"/> に <c>SampleClass</c>、<paramref name="subDirectory"/> に <c>A\B</c> を指定した場合、
        /// <c>A\B\SampleClass.resx</c> がニュートラル カルチャのリソース、<c>A\B\SampleClass.ja-JP.resx</c> がカルチャ ja-JP 向けのリソースとして認識されます)。
        /// </remarks>
        /// <typeparam name="TTarget">対象とする型。</typeparam>
        /// <param name="subDirectory"><typeparamref name="TTarget"/> が定義されたアセンブリを格納しているディレクトリから、リソースファイルを格納しているディレクトリへの相対パス。</param>
        /// <returns>読み込んだリソースを格納している <see cref="ResourceLocalizer"/>。</returns>
        public static ResourceLocalizer FromResXOfType<TTarget>(string subDirectory) => FromResXOfType(typeof(TTarget), subDirectory);


        /// <summary>
        /// リソースの一覧と、現在のカルチャを取得する方法を指定して、<see cref="ResourceLocalizer"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="resources">リソースの一覧。</param>
        /// <param name="getCurrentCultureFunc">現在のカルチャを取得するデリゲート。</param>
        protected ResourceLocalizer(Dictionary<string, Dictionary<CultureInfo, object>> resources, Func<CultureInfo> getCurrentCultureFunc)
        {
            Resources = resources;
            GetCurrentCultureFunc = getCurrentCultureFunc;
        }

        /// <summary>
        /// リソースの一覧を指定して、<see cref="ResourceLocalizer"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="resources">リソースの一覧。</param>
        protected ResourceLocalizer(Dictionary<string, Dictionary<CultureInfo, object>> resources) : this(resources, DefaultGetCurrentCultureFunc)
        {
        }


        /// <summary>
        /// 指定されたキーに紐付けられたリソースを取得します。
        /// </summary>
        /// <typeparam name="T">リソースの型。</typeparam>
        /// <param name="key">取得するリソースのキー。</param>
        /// <returns>指定されたキーに紐付けられたリソース。</returns>
        public Resource<T> Get<T>(string key)
        {
            Dictionary<CultureInfo, object> valueList = Resources[key];
            CultureInfo culture = ResolveCulture(GetCurrentCultureFunc(), valueList.Keys);

            return new Resource<T>(culture, (T)valueList[culture]);
        }

        /// <summary>
        /// 指定されたキーに紐付けられた <see cref="string"/> 型のリソースを取得します。
        /// </summary>
        /// <param name="key">取得するリソースのキー。</param>
        /// <returns>指定されたキーに紐付けられたリソース。</returns>
        public Resource<string> GetString(string key) => Get<string>(key);

        /// <summary>
        /// 指定されたキーに紐付けられた <see cref="int"/> 型のリソースを取得します。
        /// </summary>
        /// <param name="key">取得するリソースのキー。</param>
        /// <returns>指定されたキーに紐付けられたリソース。</returns>
        public Resource<int> GetInt32(string key) => Get<int>(key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="candidates"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected static CultureInfo ResolveCulture(CultureInfo culture, IEnumerable<CultureInfo> candidates)
        {
            return candidates.Contains(culture) ? culture
                : culture == CultureInfo.InvariantCulture ? throw new InvalidOperationException($"{nameof(culture)} およびその親カルチャが {nameof(candidates)} に含まれていません。")
                : ResolveCulture(culture.Parent, candidates);
        }
    }
}
