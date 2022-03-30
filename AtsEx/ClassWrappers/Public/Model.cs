using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SlimDX.Direct3D9;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal sealed class Model : ClassWrapper, IModel
    {
        static Model()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IModel>();

            MeshGetMethod = members.GetSourcePropertyGetterOf(nameof(Mesh));
            MeshSetMethod = members.GetSourcePropertySetterOf(nameof(Mesh));

            MaterialsGetMethod = members.GetSourcePropertyGetterOf(nameof(Materials));
            MaterialsSetMethod = members.GetSourcePropertySetterOf(nameof(Materials));
        }

        public Model(object src) : base(src)
        {
        }

        private static MethodInfo MeshGetMethod;
        private static MethodInfo MeshSetMethod;
        public Mesh Mesh
        {
            get => MeshGetMethod.Invoke(Src, null);
            set => MeshSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo MaterialsGetMethod;
        private static MethodInfo MaterialsSetMethod;
        private static readonly Func<object, IMaterialInfo[]> MaterialsParserToWrapper = src =>
        {
            Array srcArray = src as Array;
            MaterialInfo[] result = new MaterialInfo[srcArray.Length];
            for (int i = 0; i < srcArray.Length; i++)
            {
                object srcArrayItem = srcArray.GetValue(i);
                result[i] = srcArrayItem is null ? null : new MaterialInfo(srcArrayItem);
            }

            return result;
        };
        private static readonly Func<IMaterialInfo[], object> MaterialsParserToSource = wrapper =>
        {
            object[] result = new object[wrapper.Length];
            for (int i = 0; i < wrapper.Length; i++)
            {
                result[i] = wrapper[i]?.Src;
            }

            return result;
        };
        public IMaterialInfo[] Materials
        {
            get => MaterialsParserToWrapper(MaterialsGetMethod.Invoke(Src, null));
            set => MaterialsSetMethod.Invoke(Src, new object[] { MaterialsParserToSource(value) });
        }
    }
}
