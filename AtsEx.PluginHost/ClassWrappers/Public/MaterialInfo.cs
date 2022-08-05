using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SlimDX.Direct3D9;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// SlimDX で表示するための 2D および 3D モデルのマテリアル情報を表します。
    /// </summary>
    public sealed class MaterialInfo : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<MaterialInfo>();

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

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="MaterialInfo"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static MaterialInfo FromSource(object src)
        {
            if (src is null) return null;
            return new MaterialInfo(src);
        }

        private static MethodInfo MaterialGetMethod;
        private static MethodInfo MaterialSetMethod;
        /// <summary>
        /// マテリアルを取得・設定します。
        /// </summary>
        public Material Material
        {
            get => MaterialGetMethod.Invoke(Src, null);
            set => MaterialSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TextureGetMethod;
        private static MethodInfo TextureSetMethod;
        /// <summary>
        /// <see cref="Material"/> に関連付けるテクスチャを取得・設定します。
        /// </summary>
        public Texture Texture
        {
            get => TextureGetMethod.Invoke(Src, null);
            set => TextureSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo Is2DGetMethod;
        private static MethodInfo Is2DSetMethod;
        /// <summary>
        /// 2D モデルであるか (Z 座標を使用しないかどうか) を取得・設定します。
        /// </summary>
        public bool Is2D
        {
            get => Is2DGetMethod.Invoke(Src, null);
            set => Is2DSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
