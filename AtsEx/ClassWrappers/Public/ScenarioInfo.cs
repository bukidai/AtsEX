using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    public class ScenarioInfo : ClassWrapper, IScenarioInfo
    {
        public ScenarioInfo(object src) : base(src)
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IScenarioInfo>();

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

        protected MethodInfo PathGetMethod;
        protected MethodInfo PathSetMethod;
        public string Path
        {
            get => PathGetMethod.Invoke(Src, null);
            internal set => PathSetMethod.Invoke(Src, new object[] { value });
        }

        protected MethodInfo FileNameGetMethod;
        protected MethodInfo FileNameSetMethod;
        public string FileName
        {
            get => FileNameGetMethod.Invoke(Src, null);
            internal set => FileNameSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo DirectoryNameGetMethod;
        protected MethodInfo DirectoryNameSetMethod;
        public string DirectoryName
        {
            get => DirectoryNameGetMethod.Invoke(Src, null);
            internal set => DirectoryNameSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo TitleGetMethod;
        protected MethodInfo TitleSetMethod;
        public string Title
        {
            get => TitleGetMethod.Invoke(Src, null);
            internal set => TitleSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo ImagePathGetMethod;
        protected MethodInfo ImagePathSetMethod;
        public string ImagePath
        {
            get => ImagePathGetMethod.Invoke(Src, null);
            internal set => ImagePathSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo AuthorGetMethod;
        protected MethodInfo AuthorSetMethod;
        public string Author
        {
            get => AuthorGetMethod.Invoke(Src, null);
            internal set => AuthorSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo CommentGetMethod;
        protected MethodInfo CommentSetMethod;
        public string Comment
        {
            get => CommentGetMethod.Invoke(Src, null);
            internal set => CommentSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo RouteFilesGetMethod;
        protected MethodInfo RouteFilesSetMethod;
        public IRandomFileList RouteFiles
        {
            get => new RandomFileList(RouteFilesGetMethod.Invoke(Src, null));
            internal set => RouteFilesSetMethod.Invoke(Src, new object[] { value.Src } );
        }

        protected MethodInfo VehicleFilesGetMethod;
        protected MethodInfo VehicleFilesSetMethod;
        public IRandomFileList VehicleFiles
        {
            get => new RandomFileList(VehicleFilesGetMethod.Invoke(Src, null));
            internal set => VehicleFilesSetMethod.Invoke(Src, new object[] { value.Src } );
        }

        protected MethodInfo RouteTitleGetMethod;
        protected MethodInfo RouteTitleSetMethod;
        public string RouteTitle
        {
            get => RouteTitleGetMethod.Invoke(Src, null);
            internal set => RouteTitleSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo VehicleTitleGetMethod;
        protected MethodInfo VehicleTitleSetMethod;
        public string VehicleTitle
        {
            get => VehicleTitleGetMethod.Invoke(Src, null);
            internal set => VehicleTitleSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo ScenarioFileLoadErrorsGetMethod;
        protected MethodInfo ScenarioFileLoadErrorsSetMethod;
        public List<ILoadError> ScenarioFileLoadErrors
        {
            get
            {
                var dynamicList = ScenarioFileLoadErrorsGetMethod.Invoke(Src, null) as List<dynamic>;
                return dynamicList.ConvertAll(item => new LoadError(item) as ILoadError);
            }
            internal set
            {
                List<dynamic> dynamicList = value.ConvertAll(item => item.Src);
                ScenarioFileLoadErrorsSetMethod.Invoke(Src, new object[] { dynamicList });
            }
        }
    }
}
