using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;
using FastMember;
using ObjectiveHarmonyPatch;

namespace AtsEx.Extensions.SignalPatch
{
    /// <summary>
    /// 特定の閉塞の信号現示を自由に変更できるようにするパッチを表します。
    /// </summary>
    public sealed class SignalPatch : IDisposable
    {
        private readonly Section Target;
        private readonly HarmonyPatch HarmonyPatch;

        internal SignalPatch(FastMethod getCurrentSignalIndexMethod, SectionManager sectionManager, Section target, Converter<int, int> factory)
        {
            Target = target;

            HarmonyPatch = HarmonyPatch.Patch(getCurrentSignalIndexMethod.Source, PatchTypes.Prefix);
            HarmonyPatch.Prefix += Prefix;


            PatchInvokationResult Prefix(object sender, PatchInvokedEventArgs e)
            {
                if (e.Instance != Target.Src) return new PatchInvokationResult();

                int num = 0;
                int num2;
                while (true)
                {
                    bool flag = num >= target.SectionIndexesTrainOn.Count;
                    if (flag)
                    {
                        num2 = target.SignalIndexes.Length - 1;
                        break;
                    }
                    bool flag2 = target.SectionIndexesTrainOn[num] >= target.SectionCount;
                    if (flag2)
                    {
                        num2 = Math.Min(target.SectionIndexesTrainOn[num] - target.SectionCount, target.SignalIndexes.Length - 1);
                        break;
                    }
                    num++;
                }

                int source = target.SignalIndexes[num2];
                int converted = factory(source);

                return new PatchInvokationResult(converted, true);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            HarmonyPatch.Dispose();
        }
    }
}
