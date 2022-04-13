using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class ScenarioInfo : ClassWrapper
    {
        static ScenarioInfo()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<ScenarioInfo>();

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

        public static ScenarioInfo FromSource(object src) => new ScenarioInfo(src);

        private static MethodInfo FromFileMethod;
        public static ScenarioInfo FromFile(string path)
        {
            object src = FromFileMethod.Invoke(null, new object[] { path });
            return new ScenarioInfo(src);
        }

        private static MethodInfo PathGetMethod;
        private static MethodInfo PathSetMethod;
        public string Path
        {
            get => PathGetMethod.Invoke(Src, null);
            internal set => PathSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo FileNameGetMethod;
        private static MethodInfo FileNameSetMethod;
        public string FileName
        {
            get => FileNameGetMethod.Invoke(Src, null);
            internal set => FileNameSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo DirectoryNameGetMethod;
        private static MethodInfo DirectoryNameSetMethod;
        public string DirectoryName
        {
            get => DirectoryNameGetMethod.Invoke(Src, null);
            internal set => DirectoryNameSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo TitleGetMethod;
        private static MethodInfo TitleSetMethod;
        public string Title
        {
            get => TitleGetMethod.Invoke(Src, null);
            internal set => TitleSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo ImagePathGetMethod;
        private static MethodInfo ImagePathSetMethod;
        public string ImagePath
        {
            get => ImagePathGetMethod.Invoke(Src, null);
            internal set => ImagePathSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo AuthorGetMethod;
        private static MethodInfo AuthorSetMethod;
        public string Author
        {
            get => AuthorGetMethod.Invoke(Src, null);
            internal set => AuthorSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo CommentGetMethod;
        private static MethodInfo CommentSetMethod;
        public string Comment
        {
            get => CommentGetMethod.Invoke(Src, null);
            internal set => CommentSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo RouteFilesGetMethod;
        private static MethodInfo RouteFilesSetMethod;
        public RandomFileList RouteFiles
        {
            get => RandomFileList.FromSource(RouteFilesGetMethod.Invoke(Src, null));
            internal set => RouteFilesSetMethod.Invoke(Src, new object[] { value.Src } );
        }

        private static MethodInfo VehicleFilesGetMethod;
        private static MethodInfo VehicleFilesSetMethod;
        public RandomFileList VehicleFiles
        {
            get => RandomFileList.FromSource(VehicleFilesGetMethod.Invoke(Src, null));
            internal set => VehicleFilesSetMethod.Invoke(Src, new object[] { value.Src } );
        }

        private static MethodInfo RouteTitleGetMethod;
        private static MethodInfo RouteTitleSetMethod;
        public string RouteTitle
        {
            get => RouteTitleGetMethod.Invoke(Src, null);
            internal set => RouteTitleSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo VehicleTitleGetMethod;
        private static MethodInfo VehicleTitleSetMethod;
        public string VehicleTitle
        {
            get => VehicleTitleGetMethod.Invoke(Src, null);
            internal set => VehicleTitleSetMethod.Invoke(Src, new object[] { value } );
        }

        private static MethodInfo ScenarioFileLoadErrorsGetMethod;
        private static MethodInfo ScenarioFileLoadErrorsSetMethod;
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
