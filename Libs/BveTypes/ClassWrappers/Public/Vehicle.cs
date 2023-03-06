﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// 自車両に関する情報にアクセスするための機能を提供します。
    /// </summary>
    public class Vehicle : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<Vehicle>();

            InstrumentsGetMethod = members.GetSourcePropertyGetterOf(nameof(Instruments));
            InstrumentsSetMethod = members.GetSourcePropertySetterOf(nameof(Instruments));

            DoorsGetMethod = members.GetSourcePropertyGetterOf(nameof(Doors));
            DoorsSetMethod = members.GetSourcePropertySetterOf(nameof(Doors));

            DynamicsGetMethod = members.GetSourcePropertyGetterOf(nameof(Dynamics));
            DynamicsSetMethod = members.GetSourcePropertySetterOf(nameof(Dynamics));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="Vehicle"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected Vehicle(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Vehicle"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static Vehicle FromSource(object src) => src is null ? null : new Vehicle(src);

        private static FastMethod InstrumentsGetMethod;
        private static FastMethod InstrumentsSetMethod;
        /// <summary>
        /// 自列車を構成する機器を表す <see cref="VehicleInstrumentSet"/> を取得・設定します。
        /// </summary>
        public VehicleInstrumentSet Instruments
        {
            get => VehicleInstrumentSet.FromSource(InstrumentsGetMethod.Invoke(Src, null));
            set => InstrumentsSetMethod.Invoke(Src, new object[] { value.Src });
        }

        private static FastMethod DoorsGetMethod;
        private static FastMethod DoorsSetMethod;
        /// <summary>
        /// 自列車のドアのセットを取得・設定します。
        /// </summary>
        public DoorSet Doors
        {
            get => DoorSet.FromSource(DoorsGetMethod.Invoke(Src, null));
            set => DoorsSetMethod.Invoke(Src, new object[] { value.Src });
        }

        private static FastMethod DynamicsGetMethod;
        private static FastMethod DynamicsSetMethod;
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
