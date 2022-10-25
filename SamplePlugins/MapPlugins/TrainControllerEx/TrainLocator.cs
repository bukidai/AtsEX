using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using SlimDX.Direct3D9;

using AtsEx.PluginHost;
using AtsEx.PluginHost.ClassWrappers;

namespace AtsEx.Samples.MapPlugins.TrainControllerEx
{
    internal class TrainLocator
    {
        public Vector3 Location { get; private set; }
        public float Direction { get; private set; }

        private readonly Train Target;

        private readonly Func<float> GetBlockIndexFunc;
        private readonly Func<float> GetMaxDrawDistanceFunc;

        private readonly float RotationFactorA; // 曲がりやすさ。単位は [m/s]^-1
        private readonly float RotationFactorB; // 速度の上昇につれて曲がりづらくなる率。単位は [m/s]^-2

        private Matrix WorldMatrix = Matrix.Identity;

        public TrainLocator(Train target, Func<float> getBlockIndexFunc, Func<float> getMaxDrawDistanceFunc, Vector3 initialLocation, float initialDirection, float rotationFactorA, float rotationFactorB)
        {
            Target = target;

            GetBlockIndexFunc = getBlockIndexFunc;
            GetMaxDrawDistanceFunc = getMaxDrawDistanceFunc;

            Location = initialLocation;
            Direction = initialDirection;

            RotationFactorA = rotationFactorA;
            RotationFactorB = rotationFactorB;
        }

        public void Tick(float rotationSpeedRatio, float speed, TimeSpan elapsed)
        {
            float rotationSpeed =
                speed > 0 ?   Math.Max(0, RotationFactorA * speed - RotationFactorB * speed * speed)
                : speed < 0 ? Math.Min(0, RotationFactorA * speed + RotationFactorB * speed * speed)
                : 0;

            Direction += rotationSpeedRatio * rotationSpeed * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000;

            Matrix advanceMatrix = Matrix.Translation(0, 0, -(float)(speed * elapsed.Ticks / TimeSpan.TicksPerMillisecond / 1000));
            Matrix rotationMatrix = Matrix.RotationY(Direction);
            Matrix locatingMatrix = Matrix.Translation(Location);

            WorldMatrix = advanceMatrix * rotationMatrix * locatingMatrix;
            Location = WorldMatrix.GetTranslation();
        }

        public Action<Direct3DProvider, Matrix> CreateDrawDelegate()
        {
            return (direct3DProvider, viewMatrix) =>
            {
                Vector3 userLocation = viewMatrix.GetTranslation();
                float maxDrawDistance = GetMaxDrawDistanceFunc();

                foreach (Structure structure in Target.TrainInfo.Structures)
                {
                    if (structure.Model is null) return;
                    if (Vector3.Distance(userLocation, Location) > maxDrawDistance) return;

                    direct3DProvider.Device.SetTransform(TransformState.World, WorldMatrix * Matrix.Translation(0, 0, -25 * GetBlockIndexFunc()) * viewMatrix);
                    structure.Model.Draw(direct3DProvider, false);
                    structure.Model.Draw(direct3DProvider, true);
                }
            };
        }
    }
}
