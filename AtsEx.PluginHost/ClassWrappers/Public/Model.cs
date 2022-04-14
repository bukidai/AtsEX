using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SlimDX.Direct3D9;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class Model : ClassWrapper
    {
        static Model()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<Model>();

            MeshGetMethod = members.GetSourcePropertyGetterOf(nameof(Mesh));
            MeshSetMethod = members.GetSourcePropertySetterOf(nameof(Mesh));

            MaterialsGetMethod = members.GetSourcePropertyGetterOf(nameof(Materials));
            MaterialsSetMethod = members.GetSourcePropertySetterOf(nameof(Materials));
        }

        private Model(object src) : base(src)
        {
        }

        public static Model FromSource(object src)
        {
            if (src is null) return null;
            return new Model(src);
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
        private static readonly Func<object, MaterialInfo[]> MaterialsParserToWrapper = src =>
        {
            Array srcArray = src as Array;
            MaterialInfo[] result = new MaterialInfo[srcArray.Length];
            for (int i = 0; i < srcArray.Length; i++)
            {
                object srcArrayItem = srcArray.GetValue(i);
                result[i] = srcArrayItem is null ? null : MaterialInfo.FromSource(srcArrayItem);
            }

            return result;
        };
        private static readonly Func<MaterialInfo[], object> MaterialsParserToSource = wrapper =>
        {
            object[] result = new object[wrapper.Length];
            for (int i = 0; i < wrapper.Length; i++)
            {
                result[i] = wrapper[i]?.Src;
            }

            return result;
        };
        public MaterialInfo[] Materials
        {
            get => MaterialsParserToWrapper(MaterialsGetMethod.Invoke(Src, null));
            set => MaterialsSetMethod.Invoke(Src, new object[] { MaterialsParserToSource(value) });
        }
    }
}
