using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// シナリオ本体の詳細へアクセスするための機能を提供します。
    /// </summary>
    /// <remarks>
    /// 読み込まれたファイルに関する情報にアクセスするには <see cref="ScenarioInfo"/> を使用してください。
    /// </remarks>
    /// <seealso cref="ScenarioInfo"/>
    public sealed class Scenario : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<Scenario>();

            TimeManagerGetMethod = members.GetSourcePropertyGetterOf(nameof(TimeManager));
            LocationManagerGetMethod = members.GetSourcePropertyGetterOf(nameof(LocationManager));
            RouteGetMethod = members.GetSourcePropertyGetterOf(nameof(Route));
            VehicleGetMethod = members.GetSourcePropertyGetterOf(nameof(Vehicle));
            TimeTableGetMethod = members.GetSourcePropertyGetterOf(nameof(TimeTable));
        }

        private Scenario(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Scenario"/> クラスのインスタンス。</returns>
        public static Scenario FromSource(object src)
        {
            if (src is null) return null;
            return new Scenario(src);
        }

        private static MethodInfo TimeManagerGetMethod;
        /// <summary>
        /// このシナリオに関連付けられた <see cref="ClassWrappers.TimeManager"/> のインスタンスを取得します。
        /// </summary>
        public TimeManager TimeManager
        {
            get => ClassWrappers.TimeManager.FromSource(TimeManagerGetMethod.Invoke(Src, null));
        }

        private static MethodInfo LocationManagerGetMethod;
        /// <summary>
        /// このシナリオに関連付けられた <see cref="UserVehicleLocationManager"/> のインスタンスを取得します。
        /// </summary>
        public UserVehicleLocationManager LocationManager
        {
            get => ClassWrappers.UserVehicleLocationManager.FromSource(LocationManagerGetMethod.Invoke(Src, null));
        }

        private static MethodInfo RouteGetMethod;
        /// <summary>
        /// このシナリオに関連付けられた <see cref="ClassWrappers.Route"/> のインスタンスを取得します。
        /// </summary>
        public Route Route
        {
            get => ClassWrappers.Route.FromSource(RouteGetMethod.Invoke(Src, null));
        }

        private static MethodInfo VehicleGetMethod;
        /// <summary>
        /// このシナリオに関連付けられた <see cref="ClassWrappers.Vehicle"/> のインスタンスを取得します。
        /// </summary>
        public Vehicle Vehicle
        {
            get => ClassWrappers.Vehicle.FromSource(VehicleGetMethod.Invoke(Src, null));
        }

        private static MethodInfo TimeTableGetMethod;
        /// <summary>
        /// このシナリオに関連付けられた <see cref="ClassWrappers.TimeTable"/> のインスタンスを取得します。
        /// </summary>
        public TimeTable TimeTable
        {
            get => ClassWrappers.TimeTable.FromSource(TimeTableGetMethod.Invoke(Src, null));
        }
    }
}
