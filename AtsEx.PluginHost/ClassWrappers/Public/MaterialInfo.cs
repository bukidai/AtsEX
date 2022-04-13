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
    public sealed class MaterialInfo : ClassWrapper
    {
        static MaterialInfo()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<MaterialInfo>();

            MaterialGetMethod = members.GetSourcePropertyGetterOf(nameof(Material));
            MaterialSetMethod = members.GetSourcePropertySetterOf(nameof(Material));

            TextureGetMethod = members.GetSourcePropertyGetterOf(nameof(Texture));
            TextureSetMethod = members.GetSourcePropertySetterOf(nameof(Texture));

            Is2DGetMethod = members.GetSourcePropertyGetterOf(nameof(Is2D));
            Is2DSetMethod = members.GetSourcePropertySetterOf(nameof(Is2D));
        }

        private MaterialInfo(object src) : base(src)
        {
        }

        public static MaterialInfo FromSource(object src) => new MaterialInfo(src);

        private static MethodInfo MaterialGetMethod;
        private static MethodInfo MaterialSetMethod;
        public Material Material
        {
            get => MaterialGetMethod.Invoke(Src, null);
            set => MaterialSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TextureGetMethod;
        private static MethodInfo TextureSetMethod;
        public Texture Texture
        {
            get => TextureGetMethod.Invoke(Src, null);
            set => TextureSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo Is2DGetMethod;
        private static MethodInfo Is2DSetMethod;
        public bool Is2D
        {
            get => Is2DGetMethod.Invoke(Src, null);
            set => Is2DSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
