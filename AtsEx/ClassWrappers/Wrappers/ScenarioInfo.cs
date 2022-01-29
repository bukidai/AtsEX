using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    public class ScenarioInfo : ClassWrapper, IScenarioInfo
    {
        [UnderConstruction]
        private Assembly _assembly;

        [WillRefactor]
        public ScenarioInfo(Assembly assembly, object src) : base(src)
        {
            _assembly = assembly;

            PathGetMethod = GetMethod("i");
            PathSetMethod = GetMethod("g", typeof(string));

            FileNameGetMethod = GetMethod("d");
            FileNameSetMethod = GetMethod("i", typeof(string));

            DirectoryNameGetMethod = GetMethod("c");
            DirectoryNameSetMethod = GetMethod("c", typeof(string));

            TitleGetMethod = GetMethod("k");
            TitleSetMethod = GetMethod("d", typeof(string));

            ImagePathGetMethod = GetMethod("j");
            ImagePathSetMethod = GetMethod("f", typeof(string));

            AuthorGetMethod = GetMethod("a");
            AuthorSetMethod = GetMethod("h", typeof(string));

            CommentGetMethod = GetMethod("f");
            CommentSetMethod = GetMethod("e", typeof(string));

            RouteFilesGetMethod = GetMethod("h");
            RouteFilesSetMethod = GetMethod("b", assembly, "dr");

            VehicleFilesGetMethod = GetMethod("g");
            VehicleFilesSetMethod = GetMethod("a", assembly, "dr");

            RouteTitleGetMethod = GetMethod("b");
            RouteTitleSetMethod = GetMethod("j", typeof(string));

            VehicleTitleGetMethod = GetMethod("e");
            VehicleTitleSetMethod = GetMethod("b", typeof(string));

            ScenarioFileLoadErrorsGetMethod = GetMethod("l");
            ScenarioFileLoadErrorsSetMethod = GetMethod("a", typeof(List<>));
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
        [WillRefactor]
        public string FileName
        {
            get => FileNameGetMethod.Invoke(Src, null);
            internal set => FileNameSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo DirectoryNameGetMethod;
        protected MethodInfo DirectoryNameSetMethod;
        [WillRefactor]
        public string DirectoryName
        {
            get => DirectoryNameGetMethod.Invoke(Src, null);
            internal set => DirectoryNameSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo TitleGetMethod;
        protected MethodInfo TitleSetMethod;
        [WillRefactor]
        public string Title
        {
            get => TitleGetMethod.Invoke(Src, null);
            internal set => TitleSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo ImagePathGetMethod;
        protected MethodInfo ImagePathSetMethod;
        [WillRefactor]
        public string ImagePath
        {
            get => ImagePathGetMethod.Invoke(Src, null);
            internal set => ImagePathSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo AuthorGetMethod;
        protected MethodInfo AuthorSetMethod;
        [WillRefactor]
        public string Author
        {
            get => AuthorGetMethod.Invoke(Src, null);
            internal set => AuthorSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo CommentGetMethod;
        protected MethodInfo CommentSetMethod;
        [WillRefactor]
        public string Comment
        {
            get => CommentGetMethod.Invoke(Src, null);
            internal set => CommentSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo RouteFilesGetMethod;
        protected MethodInfo RouteFilesSetMethod;
        [WillRefactor]
        public IRandomFileList RouteFiles
        {
            get => new RandomFileList(_assembly, RouteFilesGetMethod.Invoke(Src, null));
            internal set => RouteFilesSetMethod.Invoke(Src, new object[] { value.Src } );
        }

        protected MethodInfo VehicleFilesGetMethod;
        protected MethodInfo VehicleFilesSetMethod;
        [WillRefactor]
        public IRandomFileList VehicleFiles
        {
            get => new RandomFileList(_assembly, VehicleFilesGetMethod.Invoke(Src, null));
            internal set => VehicleFilesSetMethod.Invoke(Src, new object[] { value.Src } );
        }

        protected MethodInfo RouteTitleGetMethod;
        protected MethodInfo RouteTitleSetMethod;
        [WillRefactor]
        public string RouteTitle
        {
            get => RouteTitleGetMethod.Invoke(Src, null);
            internal set => RouteTitleSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo VehicleTitleGetMethod;
        protected MethodInfo VehicleTitleSetMethod;
        [WillRefactor]
        public string VehicleTitle
        {
            get => VehicleTitleGetMethod.Invoke(Src, null);
            internal set => VehicleTitleSetMethod.Invoke(Src, new object[] { value } );
        }

        protected MethodInfo ScenarioFileLoadErrorsGetMethod;
        protected MethodInfo ScenarioFileLoadErrorsSetMethod;
        [WillRefactor]
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
