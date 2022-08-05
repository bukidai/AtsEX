using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// シナリオファイルから読み込んだ情報にアクセスするための機能を提供します。
    /// </summary>
    /// <remarks>
    /// シナリオ本体の詳細へアクセスするには <see cref="Scenario"/> を使用してください。
    /// </remarks>
    /// <seealso cref="Scenario"/>
    public class ScenarioInfo : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<ScenarioInfo>();

            FromFileMethod = members.GetSourceMethodOf(nameof(FromFile));

            PathGetMethod = members.GetSourcePropertyGetterOf(nameof(Path));
            PathSetMethod = members.GetSourcePropertySetterOf(nameof(Path));

            FileNameGetMethod = members.GetSourcePropertyGetterOf(nameof(FileName));
            FileNameSetMethod = members.GetSourcePropertySetterOf(nameof(FileName));

            DirectoryNameGetMethod = members.GetSourcePropertyGetterOf(nameof(DirectoryName));
            DirectoryNameSetMethod = members.GetSourcePropertySetterOf(nameof(DirectoryName));

            TitleGetMethod = members.GetSourcePropertyGetterOf(nameof(Title));
            TitleSetMethod = members.GetSourcePropertySetterOf(nameof(Title));

            ImagePathGetMethod = members.GetSourcePropertyGetterOf(nameof(ImagePath));
            ImagePathSetMethod = members.GetSourcePropertySetterOf(nameof(ImagePath));

            AuthorGetMethod = members.GetSourcePropertyGetterOf(nameof(Author));
            AuthorSetMethod = members.GetSourcePropertySetterOf(nameof(Author));

            CommentGetMethod = members.GetSourcePropertyGetterOf(nameof(Comment));
            CommentSetMethod = members.GetSourcePropertySetterOf(nameof(Comment));

            RouteFilesGetMethod = members.GetSourcePropertyGetterOf(nameof(RouteFiles));
            RouteFilesSetMethod = members.GetSourcePropertySetterOf(nameof(RouteFiles));

            VehicleFilesGetMethod = members.GetSourcePropertyGetterOf(nameof(VehicleFiles));
            VehicleFilesSetMethod = members.GetSourcePropertySetterOf(nameof(VehicleFiles));

            RouteTitleGetMethod = members.GetSourcePropertyGetterOf(nameof(RouteTitle));
            RouteTitleSetMethod = members.GetSourcePropertySetterOf(nameof(RouteTitle));

            VehicleTitleGetMethod = members.GetSourcePropertyGetterOf(nameof(VehicleTitle));
            VehicleTitleSetMethod = members.GetSourcePropertySetterOf(nameof(VehicleTitle));

            ScenarioFileLoadErrorsGetMethod = members.GetSourcePropertyGetterOf(nameof(ScenarioFileLoadErrors));
            ScenarioFileLoadErrorsSetMethod = members.GetSourcePropertySetterOf(nameof(ScenarioFileLoadErrors));
        }

        protected ScenarioInfo(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="ScenarioInfo"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static ScenarioInfo FromSource(object src)
        {
            if (src is null) return null;
            return new ScenarioInfo(src);
        }

        private static MethodInfo FromFileMethod;
        /// <summary>
        /// 読み込むシナリオファイルを指定して <see cref="ScenarioInfo"/> クラスのインスタンスを生成します。
        /// </summary>
        /// <param name="path">読み込むシナリオファイルのパス。</param>
        /// <returns>シナリオファイルから読み込んだ情報を表す <see cref="ScenarioInfo"/>。</returns>
        public static ScenarioInfo FromFile(string path)
        {
            object src = FromFileMethod.Invoke(null, new object[] { path });
            return new ScenarioInfo(src);
        }

        private static MethodInfo PathGetMethod;
        private static MethodInfo PathSetMethod;
        /// <summary>
        /// シナリオファイルのパスを取得します。
        /// </summary>
        public string Path
        {
            get => PathGetMethod.Invoke(Src, null);
            internal set => PathSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo FileNameGetMethod;
        private static MethodInfo FileNameSetMethod;
        /// <summary>
        /// シナリオファイルのファイル名を取得します。
        /// </summary>
        public string FileName
        {
            get => FileNameGetMethod.Invoke(Src, null);
            internal set => FileNameSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo DirectoryNameGetMethod;
        private static MethodInfo DirectoryNameSetMethod;
        /// <summary>
        /// シナリオファイルのディレクトリ情報を取得します。
        /// </summary>
        public string DirectoryName
        {
            get => DirectoryNameGetMethod.Invoke(Src, null);
            internal set => DirectoryNameSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo TitleGetMethod;
        private static MethodInfo TitleSetMethod;
        /// <summary>
        /// シナリオのタイトルを取得します。
        /// </summary>
        public string Title
        {
            get => TitleGetMethod.Invoke(Src, null);
            internal set => TitleSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo ImagePathGetMethod;
        private static MethodInfo ImagePathSetMethod;
        /// <summary>
        /// シナリオのサムネイル画像のパスを取得します。
        /// </summary>
        public string ImagePath
        {
            get => ImagePathGetMethod.Invoke(Src, null);
            internal set => ImagePathSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo AuthorGetMethod;
        private static MethodInfo AuthorSetMethod;
        /// <summary>
        /// シナリオの作者を取得します。
        /// </summary>
        public string Author
        {
            get => AuthorGetMethod.Invoke(Src, null);
            internal set => AuthorSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo CommentGetMethod;
        private static MethodInfo CommentSetMethod;
        /// <summary>
        /// シナリオの説明を取得します。
        /// </summary>
        public string Comment
        {
            get => CommentGetMethod.Invoke(Src, null);
            internal set => CommentSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo RouteFilesGetMethod;
        private static MethodInfo RouteFilesSetMethod;
        /// <summary>
        /// 選択されうるマップファイルのリストを取得します。
        /// </summary>
        public RandomFileList RouteFiles
        {
            get => RandomFileList.FromSource(RouteFilesGetMethod.Invoke(Src, null));
            internal set => RouteFilesSetMethod.Invoke(Src, new object[] { value.Src } );
        }

        private static MethodInfo VehicleFilesGetMethod;
        private static MethodInfo VehicleFilesSetMethod;
        /// <summary>
        /// 選択されうる車両ファイルのリストを取得します。
        /// </summary>
        public RandomFileList VehicleFiles
        {
            get => RandomFileList.FromSource(VehicleFilesGetMethod.Invoke(Src, null));
            internal set => VehicleFilesSetMethod.Invoke(Src, new object[] { value.Src } );
        }

        private static MethodInfo RouteTitleGetMethod;
        private static MethodInfo RouteTitleSetMethod;
        /// <summary>
        /// 路線名を取得します。
        /// </summary>
        public string RouteTitle
        {
            get => RouteTitleGetMethod.Invoke(Src, null);
            internal set => RouteTitleSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo VehicleTitleGetMethod;
        private static MethodInfo VehicleTitleSetMethod;
        /// <summary>
        /// 車両名を取得します。
        /// </summary>
        public string VehicleTitle
        {
            get => VehicleTitleGetMethod.Invoke(Src, null);
            internal set => VehicleTitleSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo ScenarioFileLoadErrorsGetMethod;
        private static MethodInfo ScenarioFileLoadErrorsSetMethod;
        /// <summary>
        /// シナリオファイルの読込時に発生した、継続不能となる深刻なエラーを取得します。
        /// </summary>
        /// <remarks>
        /// このプロパティは正常に動作しません。
        /// </remarks>
        [UnderConstruction]
        public List<LoadError> ScenarioFileLoadErrors
        {
            get
            {
                dynamic list = ScenarioFileLoadErrorsGetMethod.Invoke(Src, null);
                return list.ConvertAll<LoadError>(new Converter<dynamic, LoadError>(item => LoadError.FromSource(item)));
            }
            internal set
            {
                List<dynamic> list = value.ConvertAll(item => item.Src);
                ScenarioFileLoadErrorsSetMethod.Invoke(Src, new object[] { list });
            }
        }
    }
}
