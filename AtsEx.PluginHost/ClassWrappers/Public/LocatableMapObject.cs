using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using SlimDX.Direct3D9;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class LocatableMapObject : MapObjectBase
    {
        static LocatableMapObject()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<LocatableMapObject>();

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

        protected LocatableMapObject(double location, string trackKey, double x, double y, double z, double dx, double dy, double dz, TiltOptions tiltOptions, double span)
            : this(location, trackKey, x, y, z, dx, dy, dz, (int)tiltOptions, span)
        {
        }

        private static ConstructorInfo Constructor2;
        private LocatableMapObject(double location, string trackKey, int tilt, double span)
            : this(Constructor2.Invoke(new object[] { location, trackKey, tilt, span }))
        {
        }

        protected LocatableMapObject(double location, string trackKey, TiltOptions tiltOptions, double span)
            : this(location, trackKey, (int)tiltOptions, span)
        {
        }

        private static MethodInfo MatrixGetMethod;
        private static MethodInfo MatrixSetMethod;
        public Matrix Matrix
        {
            get => MatrixGetMethod.Invoke(Src, null);
            set => MatrixSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TrackKeyGetMethod;
        private static MethodInfo TrackKeySetMethod;
        public string TrackKey
        {
            get => TrackKeyGetMethod.Invoke(Src, null);
            set => TrackKeySetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo SpanGetMethod;
        private static MethodInfo SpanSetMethod;
        public double Span
        {
            get => SpanGetMethod.Invoke(Src, null);
            set => SpanSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TiltsAlongCantGetMethod;
        private static MethodInfo TiltsAlongCantSetMethod;
        public bool TiltsAlongCant
        {
            get => TiltsAlongCantGetMethod.Invoke(Src, null);
            set => TiltsAlongCantSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TiltsAlongGradientGetMethod;
        private static MethodInfo TiltsAlongGradientSetMethod;
        public bool TiltsAlongGradient
        {
            get => TiltsAlongGradientGetMethod.Invoke(Src, null);
            set => TiltsAlongGradientSetMethod.Invoke(Src, new object[] { value });
        }

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
