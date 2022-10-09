using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 自列車を構成する機器のセットを表します。
    /// </summary>
    public class VehicleInstrumentSet : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<VehicleInstrumentSet>();

            CabGetMethod = members.GetSourcePropertyGetterOf(nameof(Cab));
            CabSetMethod = members.GetSourcePropertySetterOf(nameof(Cab));

            PluginLoaderGetMethod = members.GetSourcePropertyGetterOf(nameof(PluginLoader));
            PluginLoaderSetMethod = members.GetSourcePropertySetterOf(nameof(PluginLoader));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="VehicleInstrumentSet"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected VehicleInstrumentSet(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="VehicleInstrumentSet"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static VehicleInstrumentSet FromSource(object src) => src is null ? null : new VehicleInstrumentSet(src);

        private static FastMethod CabGetMethod;
        private static FastMethod CabSetMethod;
        /// <summary>
        /// 運転台のハンドルを表す <see cref="CabBase"/> を取得します。
        /// </summary>
        public CabBase Cab
        {
            get => CabBase.FromSource(CabGetMethod.Invoke(Src, null));
            internal set => CabSetMethod.Invoke(Src, new object[] { value.Src });
        }

        private static FastMethod PluginLoaderGetMethod;
        private static FastMethod PluginLoaderSetMethod;
        /// <summary>
        /// プラグインの読込機能を提供する <see cref="ClassWrappers.PluginLoader"/> を取得します。
        /// </summary>
        public PluginLoader PluginLoader
        {
            get => ClassWrappers.PluginLoader.FromSource(PluginLoaderGetMethod.Invoke(Src, null));
            internal set => PluginLoaderSetMethod.Invoke(Src, new object[] { value.Src });
        }
    }
}
