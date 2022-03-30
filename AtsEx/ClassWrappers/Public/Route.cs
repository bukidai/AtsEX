using System;
using System.Collections;
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
    internal sealed class Route : ClassWrapper, PluginHost.ClassWrappers.IRoute
    {
        static Route()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<PluginHost.ClassWrappers.IRoute>();

            DrawLimitLocationGetMethod = members.GetSourcePropertyGetterOf(nameof(DrawLimitLocation));
            DrawLimitLocationSetMethod = members.GetSourcePropertySetterOf(nameof(DrawLimitLocation));

            StructuresGetMethod = members.GetSourcePropertyGetterOf(nameof(Structures));

            SoundsGetMethod = members.GetSourcePropertyGetterOf(nameof(Sounds));

            Sounds3DGetMethod = members.GetSourcePropertyGetterOf(nameof(Sounds3D));

            StructureModelsGetMethod = members.GetSourcePropertyGetterOf(nameof(StructureModels));
        }

        public Route(object src) : base(src)
        {
        }

        private static MethodInfo DrawLimitLocationGetMethod;
        private static MethodInfo DrawLimitLocationSetMethod;
        public double DrawLimitLocation
        {
            get => DrawLimitLocationGetMethod.Invoke(Src, null);
            set => DrawLimitLocationSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo StructuresGetMethod;
        public IStructureSet Structures
        {
            get => new StructureSet(StructuresGetMethod.Invoke(Src, null));
        }

        private static MethodInfo SoundsGetMethod;
        private static readonly Func<object, ISound> SoundsParserToWrapper = src => src is null ? null : new Sound(src);
        public IWrappedSortedList<string, ISound> Sounds
        {
            get
            {
                IDictionary dictionarySrc = SoundsGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, ISound>(dictionarySrc, SoundsParserToWrapper);
            }
        }

        private static MethodInfo Sounds3DGetMethod;
        private static readonly Func<object, ISound[]> Sounds3DParserToWrapper = src =>
        {
            Array srcArray = src as Array;
            Sound[] result = new Sound[srcArray.Length];
            for (int i = 0; i < srcArray.Length; i++)
            {
                object srcArrayItem = srcArray.GetValue(i);
                result[i] = srcArrayItem is null ? null : new Sound(srcArrayItem);
            }

            return result;
        };
        private static readonly Func<ISound[], object> Sounds3DParserToSource = wrapper =>
        {
            object[] result = new object[wrapper.Length];
            for (int i = 0; i < wrapper.Length; i++)
            {
                result[i] = wrapper[i]?.Src;
            }

            return result;
        };
        public IWrappedSortedList<string, ISound[]> Sounds3D
        {
            get
            {
                IDictionary dictionarySrc = Sounds3DGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, ISound[]>(dictionarySrc, Sounds3DParserToWrapper, Sounds3DParserToSource);
            }
        }

        private static MethodInfo StructureModelsGetMethod;
        private static readonly Func<object, IModel> StructureModelsParserToWrapper = src => src is null ? null : new Model(src);
        public IWrappedSortedList<string, IModel> StructureModels
        {
            get
            {
                IDictionary dictionarySrc = StructureModelsGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, IModel>(dictionarySrc, StructureModelsParserToWrapper);
            }
        }
    }
}
