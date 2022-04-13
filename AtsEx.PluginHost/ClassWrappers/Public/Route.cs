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
    public sealed class Route : ClassWrapper
    {
        static Route()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<Route>();

            DrawLimitLocationGetMethod = members.GetSourcePropertyGetterOf(nameof(DrawLimitLocation));
            DrawLimitLocationSetMethod = members.GetSourcePropertySetterOf(nameof(DrawLimitLocation));

            StructuresGetMethod = members.GetSourcePropertyGetterOf(nameof(Structures));

            StationsGetMethod = members.GetSourcePropertyGetterOf(nameof(Stations));

            SoundsGetMethod = members.GetSourcePropertyGetterOf(nameof(Sounds));

            Sounds3DGetMethod = members.GetSourcePropertyGetterOf(nameof(Sounds3D));

            StructureModelsGetMethod = members.GetSourcePropertyGetterOf(nameof(StructureModels));
        }

        private Route(object src) : base(src)
        {
        }

        public static Route FromSource(object src) => new Route(src);

        private static MethodInfo DrawLimitLocationGetMethod;
        private static MethodInfo DrawLimitLocationSetMethod;
        public double DrawLimitLocation
        {
            get => DrawLimitLocationGetMethod.Invoke(Src, null);
            set => DrawLimitLocationSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo StructuresGetMethod;
        public StructureSet Structures
        {
            get => StructureSet.FromSource(StructuresGetMethod.Invoke(Src, null));
        }

        private static MethodInfo StationsGetMethod;
        public StationList Stations
        {
            get => StationList.FromSource(StationsGetMethod.Invoke(Src, null));
        }

        private static MethodInfo SoundsGetMethod;
        private static readonly Func<object, Sound> SoundsParserToWrapper = src => src is null ? null : Sound.FromSource(src);
        public WrappedSortedList<string, Sound> Sounds
        {
            get
            {
                IDictionary dictionarySrc = SoundsGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, Sound>(dictionarySrc, SoundsParserToWrapper);
            }
        }

        private static MethodInfo Sounds3DGetMethod;
        private static readonly Func<object, Sound[]> Sounds3DParserToWrapper = src =>
        {
            Array srcArray = src as Array;
            Sound[] result = new Sound[srcArray.Length];
            for (int i = 0; i < srcArray.Length; i++)
            {
                object srcArrayItem = srcArray.GetValue(i);
                result[i] = srcArrayItem is null ? null : Sound.FromSource(srcArrayItem);
            }

            return result;
        };
        private static readonly Func<Sound[], object> Sounds3DParserToSource = wrapper =>
        {
            object[] result = new object[wrapper.Length];
            for (int i = 0; i < wrapper.Length; i++)
            {
                result[i] = wrapper[i]?.Src;
            }

            return result;
        };
        public WrappedSortedList<string, Sound[]> Sounds3D
        {
            get
            {
                IDictionary dictionarySrc = Sounds3DGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, Sound[]>(dictionarySrc, Sounds3DParserToWrapper, Sounds3DParserToSource);
            }
        }

        private static MethodInfo StructureModelsGetMethod;
        private static readonly Func<object, Model> StructureModelsParserToWrapper = src => src is null ? null : Model.FromSource(src);
        public WrappedSortedList<string, Model> StructureModels
        {
            get
            {
                IDictionary dictionarySrc = StructureModelsGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, Model>(dictionarySrc, StructureModelsParserToWrapper);
            }
        }
    }
}
