using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class PutBetweenStructure : MapObjectBase
    {
        static PutBetweenStructure()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<PutBetweenStructure>();

            Constructor = members.GetSourceConstructor();

            ModelGetMethod = members.GetSourcePropertyGetterOf(nameof(Model));
            ModelSetMethod = members.GetSourcePropertySetterOf(nameof(Model));

            TrackKey1GetMethod = members.GetSourcePropertyGetterOf(nameof(TrackKey1));
            TrackKey1SetMethod = members.GetSourcePropertySetterOf(nameof(TrackKey1));

            TrackKey2GetMethod = members.GetSourcePropertyGetterOf(nameof(TrackKey2));
            TrackKey2SetMethod = members.GetSourcePropertySetterOf(nameof(TrackKey2));

            TransformOnlyXGetMethod = members.GetSourcePropertyGetterOf(nameof(TransformOnlyX));
        }

        protected PutBetweenStructure(object src) : base(src)
        {
        }

        public static PutBetweenStructure FromSource(object src)
        {
            if (src is null) return null;
            return new PutBetweenStructure(src);
        }

        private static ConstructorInfo Constructor;
        public PutBetweenStructure(double location, string trackKey1, string trackKey2, Model model, bool transformOnlyX)
            : this(Constructor.Invoke(new object[] { location, trackKey1, trackKey2, model, transformOnlyX }))
        {
        }

        private static MethodInfo ModelGetMethod;
        private static MethodInfo ModelSetMethod;
        public Model Model
        {
            get => ModelGetMethod.Invoke(Src, null);
            set => ModelSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TrackKey1GetMethod;
        private static MethodInfo TrackKey1SetMethod;
        public string TrackKey1
        {
            get => TrackKey1GetMethod.Invoke(Src, null);
            set => TrackKey1SetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TrackKey2GetMethod;
        private static MethodInfo TrackKey2SetMethod;
        public string TrackKey2
        {
            get => TrackKey2GetMethod.Invoke(Src, null);
            set => TrackKey2SetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TransformOnlyXGetMethod;
        public bool TransformOnlyX
        {
            get => TransformOnlyXGetMethod.Invoke(Src, null);
        }
    }
}
