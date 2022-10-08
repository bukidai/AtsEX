using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using SlimDX.Direct3D9;

using FastMember;
using ObjectiveHarmonyPatch;
using TypeWrapping;

using Automatic9045.AtsEx.PluginHost.BveTypes;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.Extensions
{
    /// <summary>
    /// 他列車を自由に移動・回転できるようにするパッチを表します。
    /// </summary>
    public sealed class TrainDrawPatch : IDisposable
    {
        private static FastMethod DrawCarsMethod;

        private readonly Train Target;
        private readonly HarmonyPatch HarmonyPatch;

        [SetBveTypes]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<Train>();
            DrawCarsMethod = members.GetSourceMethodOf(nameof(Train.DrawCars));
        }

        private TrainDrawPatch(Train target, Action<Direct3DProvider, Matrix> overrideAction)
        {
            Target = target;

            HarmonyPatch = HarmonyPatch.PatchAsync(DrawCarsMethod.Source, PatchTypes.Prefix).Result;
            HarmonyPatch.Prefix += Prefix;


            PatchInvokationResult Prefix(object sender, PatchInvokedEventArgs e)
            {
                if (e.Instance != Target.Src) return new PatchInvokationResult();

                Direct3DProvider direct3DProvider = Direct3DProvider.FromSource(e.Args[0]);
                Matrix viewMatrix = (Matrix)e.Args[1];

                overrideAction(direct3DProvider, viewMatrix);

                return new PatchInvokationResult(true);
            }
        }

        private TrainDrawPatch(Train target, IMatrixConverter worldMatrixConverter, IMatrixConverter viewMatrixConverter)
            : this(target, (direct3DProvider, viewMatrix) => DrawCars(direct3DProvider, viewMatrix, target, worldMatrixConverter, viewMatrixConverter))
        {
        }

        private static void DrawCars(Direct3DProvider direct3DProvider, Matrix viewMatrix, Train target, IMatrixConverter worldMatrixConverter, IMatrixConverter viewMatrixConverter)
        {
            int distance = (int)Math.Floor(target.Location - target.UserVehicleLocationManager.Location);

            foreach (Structure structure in target.TrainInfo.Structures)
            {
                bool isTooFarInFront = distance + structure.Location >= target.DrawDistanceManager.FrontDrawDistance + 25.0;
                bool isTooFarInBack = distance + structure.Location + structure.Span < -target.DrawDistanceManager.BackDrawDistance - 25.0;

                if (!(structure.Model is null) && !isTooFarInFront && !isTooFarInBack)
                {
                    structure.TrackKey = target.TrainInfo.TrackKey;
                    Matrix trackMatrix = target.Route.GetTrackMatrix(structure, target.Location + structure.Location, (double)(target.UserVehicleLocationManager.BlockIndex * 25));
                    direct3DProvider.Device.SetTransform(TransformState.World, worldMatrixConverter.Convert(trackMatrix) * viewMatrixConverter.Convert(viewMatrix));
                    structure.Model.Draw(direct3DProvider, false);
                    structure.Model.Draw(direct3DProvider, true);
                }
            }
        }

        /// <summary>
        /// 指定した他列車に、自由に移動・回転できるようにするパッチを適用します。
        /// </summary>
        /// <param name="target">パッチを適用する他列車。</param>
        /// <param name="worldMatrixConverter">本来の配置先に設置するワールド変換行列から、実際に使用する行列に変換するためのコンバーター。</param>
        /// <returns>パッチを表す <see cref="TrainDrawPatch"/>。</returns>
        public static TrainDrawPatch Patch(Train target, IMatrixConverter worldMatrixConverter)
            => new TrainDrawPatch(target, worldMatrixConverter, new DefaultMatrixConverter());

        /// <summary>
        /// 指定した他列車に、自由に移動・回転できるようにするパッチを適用します。
        /// </summary>
        /// <param name="target">パッチを適用する他列車。</param>
        /// <param name="worldMatrixConverter">本来の配置先に設置するワールド変換行列から、実際に使用する行列に変換するためのコンバーター。</param>
        /// <param name="viewMatrixConverter">本来の配置先に設置するビュー変換行列から、実際に使用する行列に変換するためのコンバーター。</param>
        /// <returns>パッチを表す <see cref="TrainDrawPatch"/>。</returns>
        public static TrainDrawPatch Patch(Train target, IMatrixConverter worldMatrixConverter, IMatrixConverter viewMatrixConverter)
            => new TrainDrawPatch(target, worldMatrixConverter, viewMatrixConverter);

        /// <summary>
        /// 指定した他列車に、自由に移動・回転できるようにするパッチを適用します。
        /// </summary>
        /// <param name="target">パッチを適用する他列車。</param>
        /// <param name="overrideAction"><see cref="Train.DrawCars(Direct3DProvider, Matrix)"/> メソッドの処理をオーバーライドするデリゲート。</param>
        /// <returns>パッチを表す <see cref="TrainDrawPatch"/>。</returns>
        public static TrainDrawPatch Patch(Train target, Action<Direct3DProvider, Matrix> overrideAction)
            => new TrainDrawPatch(target, overrideAction);

        /// <inheritdoc/>
        public void Dispose()
        {
            HarmonyPatch.Dispose();
        }


        private class DefaultMatrixConverter : IMatrixConverter
        {
            public Matrix Convert(Matrix source) => source;
        }
    }
}
