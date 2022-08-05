using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using SlimDX.Direct3D9;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 基準となる距離程・軌道からの変位を指定するマップ オブジェクトを表します。
    /// </summary>
    /// <seealso cref="Structure"/>
    public class LocatableMapObject : MapObjectBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<LocatableMapObject>();

            Constructor1 = members.GetSourceConstructor(new Type[] { typeof(double), typeof(string), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(int), typeof(double) });
            Constructor2 = members.GetSourceConstructor(new Type[] { typeof(double), typeof(string), typeof(int), typeof(double) });

            MatrixGetMethod = members.GetSourcePropertyGetterOf(nameof(Matrix));
            MatrixSetMethod = members.GetSourcePropertySetterOf(nameof(Matrix));

            TrackKeyGetMethod = members.GetSourcePropertyGetterOf(nameof(TrackKey));
            TrackKeySetMethod = members.GetSourcePropertySetterOf(nameof(TrackKey));

            SpanGetMethod = members.GetSourcePropertyGetterOf(nameof(Span));
            SpanSetMethod = members.GetSourcePropertySetterOf(nameof(Span));

            TiltsAlongCantGetMethod = members.GetSourcePropertyGetterOf(nameof(TiltsAlongCant));
            TiltsAlongCantSetMethod = members.GetSourcePropertySetterOf(nameof(TiltsAlongCant));

            TiltsAlongGradientGetMethod = members.GetSourcePropertyGetterOf(nameof(TiltsAlongGradient));
            TiltsAlongGradientSetMethod = members.GetSourcePropertySetterOf(nameof(TiltsAlongGradient));
        }

        protected LocatableMapObject(object src) : base(src)
        {
        }

        private static ConstructorInfo Constructor1;
        private LocatableMapObject(double location, string trackKey, double x, double y, double z, double dx, double dy, double dz, int tilt, double span)
            : this(Constructor1.Invoke(new object[] { location, trackKey, x, y, z, dx, dy, dz, tilt, span }))
        {
        }

        /// <summary>
        /// 距離程、設置先の軌道、軌道からの変位、傾斜オプション、弦の長さを指定して <see cref="LocatableMapObject"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="location">設置する距離程 [m]。</param>
        /// <param name="trackKey">設置先の軌道名。</param>
        /// <param name="x">軌道からの x 座標 [m]。</param>
        /// <param name="y">軌道からの y 座標 [m]。</param>
        /// <param name="z">軌道からの z 座標 [m]。</param>
        /// <param name="dx">軌道に対する x 軸回りの角 [deg]。</param>
        /// <param name="dy">軌道に対する y 軸回りの角 [deg]。</param>
        /// <param name="dz">軌道に対する z 軸回りの角 [deg]。</param>
        /// <param name="tiltOptions">傾斜オプション。</param>
        /// <param name="span">曲線における弦の長さ [m]。</param>
        protected LocatableMapObject(double location, string trackKey, double x, double y, double z, double dx, double dy, double dz, TiltOptions tiltOptions, double span)
            : this(location, trackKey, x, y, z, dx, dy, dz, (int)tiltOptions, span)
        {
        }

        private static ConstructorInfo Constructor2;
        private LocatableMapObject(double location, string trackKey, int tilt, double span)
            : this(Constructor2.Invoke(new object[] { location, trackKey, tilt, span }))
        {
        }

        /// <summary>
        /// 距離程、設置先の軌道、傾斜オプション、弦の長さを指定して <see cref="LocatableMapObject"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="location">設置する距離程 [m]。</param>
        /// <param name="trackKey">設置先の軌道名。</param>
        /// <param name="tiltOptions">傾斜オプション。</param>
        /// <param name="span">曲線における弦の長さ [m]。</param>
        protected LocatableMapObject(double location, string trackKey, TiltOptions tiltOptions, double span)
            : this(location, trackKey, (int)tiltOptions, span)
        {
        }

        private static MethodInfo MatrixGetMethod;
        private static MethodInfo MatrixSetMethod;
        /// <summary>
        /// 軌道からの変位を表す <see cref="SlimDX.Matrix"/> を取得・設定します。<see cref="SlimDX.Matrix"/> の単位は [m] です。
        /// </summary>
        public Matrix Matrix
        {
            get => MatrixGetMethod.Invoke(Src, null);
            set => MatrixSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TrackKeyGetMethod;
        private static MethodInfo TrackKeySetMethod;
        /// <summary>
        /// 設置先の軌道名を取得・設定します。
        /// </summary>
        public string TrackKey
        {
            get => TrackKeyGetMethod.Invoke(Src, null);
            set => TrackKeySetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo SpanGetMethod;
        private static MethodInfo SpanSetMethod;
        /// <summary>
        /// 曲線における弦の長さ [m] を取得・設定します。
        /// </summary>
        public double Span
        {
            get => SpanGetMethod.Invoke(Src, null);
            set => SpanSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TiltsAlongCantGetMethod;
        private static MethodInfo TiltsAlongCantSetMethod;
        /// <summary>
        /// カントに連動して傾斜させるかどうかを取得・設定します。
        /// </summary>
        public bool TiltsAlongCant
        {
            get => TiltsAlongCantGetMethod.Invoke(Src, null);
            set => TiltsAlongCantSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TiltsAlongGradientGetMethod;
        private static MethodInfo TiltsAlongGradientSetMethod;
        /// <summary>
        /// 勾配に連動して傾斜させるかどうかを取得・設定します。
        /// </summary>
        public bool TiltsAlongGradient
        {
            get => TiltsAlongGradientGetMethod.Invoke(Src, null);
            set => TiltsAlongGradientSetMethod.Invoke(Src, new object[] { value });
        }

        /// <summary>
        /// マップ オブジェクトを傾斜させる方法を取得・設定します。
        /// </summary>
        public TiltOptions TiltOptions
        {
            get
            {
                TiltOptions result = TiltOptions.Default;
                if (TiltsAlongGradient) result |= TiltOptions.TiltsAlongGradient;
                if (TiltsAlongCant) result |= TiltOptions.TiltsAlongCant;

                return result;
            }
            set
            {
                TiltsAlongGradient = TiltOptions.HasFlag(TiltOptions.TiltsAlongGradient);
                TiltsAlongCant = TiltOptions.HasFlag(TiltOptions.TiltsAlongCant);
            }
        }
    }
}
