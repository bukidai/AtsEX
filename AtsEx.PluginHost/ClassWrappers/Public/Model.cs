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
    /// SlimDX で表示するための 2D および 3D モデルを表します。
    /// </summary>
    public sealed class Model : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<Model>();

            MeshGetMethod = members.GetSourcePropertyGetterOf(nameof(Mesh));
            MeshSetMethod = members.GetSourcePropertySetterOf(nameof(Mesh));

            MaterialsGetMethod = members.GetSourcePropertyGetterOf(nameof(Materials));
            MaterialsSetMethod = members.GetSourcePropertySetterOf(nameof(Materials));
        }

        private Model(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Model"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static Model FromSource(object src) => src is null ? null : new Model(src);

        private static MethodInfo MeshGetMethod;
        private static MethodInfo MeshSetMethod;
        /// <summary>
        /// モデルを構成する <see cref="Mesh"/> を取得・設定します。
        /// </summary>
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
        /// <summary>
        /// モデルのマテリアル情報の一覧を取得・設定します。
        /// </summary>
        public MaterialInfo[] Materials
        {
            get => MaterialsParserToWrapper(MaterialsGetMethod.Invoke(Src, null));
            set => MaterialsSetMethod.Invoke(Src, new object[] { MaterialsParserToSource(value) });
        }
    }
}
