using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 自車両に関する情報にアクセスするための機能を提供します。
    /// </summary>
    public sealed class Vehicle : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberSet members = BveTypeSet.Instance.GetClassInfoOf<Vehicle>();

            DynamicsGetMethod = members.GetSourcePropertyGetterOf(nameof(Dynamics));
            DynamicsSetMethod = members.GetSourcePropertySetterOf(nameof(Dynamics));
        }

        private Vehicle(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Vehicle"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static Vehicle FromSource(object src)
        {
            if (src is null) return null;
            return new Vehicle(src);
        }

        private static MethodInfo DynamicsGetMethod;
        private static MethodInfo DynamicsSetMethod;
        /// <summary>
        /// 曲線抵抗の係数を取得・設定します。
        /// </summary>
        public VehicleDynamics Dynamics
        {
            get => VehicleDynamics.FromSource(DynamicsGetMethod.Invoke(Src, null));
            set => DynamicsSetMethod.Invoke(Src, new object[] { value.Src });
        }
    }
}
