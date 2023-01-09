using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using SlimDX.Direct3D9;

using BveTypes.ClassWrappers;
using FastMember;
using ObjectiveHarmonyPatch;

namespace AtsEx.Extensions.PreTrainPatch
{
    /// <summary>
    /// 先行列車の走行位置を自由に変更できるようにするパッチを表します。
    /// </summary>
    public sealed class PreTrainPatch
    {
        private readonly SectionManager SectionManager;
        private readonly HarmonyPatch HarmonyPatch;

        internal PreTrainPatch(FastMethod updatePreTrainLocationMethod, SectionManager sectionManager, Action overrideAction)
        {
            SectionManager = sectionManager;

            HarmonyPatch = HarmonyPatch.Patch(updatePreTrainLocationMethod.Source, PatchTypes.Prefix);
            HarmonyPatch.Prefix += Prefix;


            PatchInvokationResult Prefix(object sender, PatchInvokedEventArgs e)
            {
                if (e.Instance != SectionManager.Src) return new PatchInvokationResult();

                overrideAction();

                return new PatchInvokationResult(true);
            }
        }

        internal PreTrainPatch(FastMethod updatePreTrainLocationMethod, SectionManager sectionManager, IPreTrainLocationConverter converter)
            : this(updatePreTrainLocationMethod, sectionManager, () => UpdatePreTrainLocation(sectionManager, converter))
        {
        }

        private static void UpdatePreTrainLocation(SectionManager sectionManager, IPreTrainLocationConverter converter)
		{
			MapObjectList preTrainPassObjects = sectionManager.PreTrainPassObjects;

            PreTrainLocation source;
            if (preTrainPassObjects.Count == 0 || sectionManager.Sections.Count == 0)
            {
                source = new PreTrainLocation(-1, sectionManager.Sections.Count - 1);
            }
            else
            {
                int timeMilliseconds = sectionManager.TimeManager.TimeMilliseconds;

                while (preTrainPassObjects.CurrentIndex >= 0 && ((ValueNode<int>)preTrainPassObjects[preTrainPassObjects.CurrentIndex]).Value > timeMilliseconds)
                {
                    preTrainPassObjects.CurrentIndex--;
                }
                while (preTrainPassObjects.CurrentIndex + 1 < preTrainPassObjects.Count && ((ValueNode<int>)preTrainPassObjects[preTrainPassObjects.CurrentIndex + 1]).Value <= timeMilliseconds)
                {
                    preTrainPassObjects.CurrentIndex++;
                }

                ValueNode<int> currentObject = (ValueNode<int>)preTrainPassObjects[preTrainPassObjects.CurrentIndex];
                ValueNode<int> nextObject = (ValueNode<int>)preTrainPassObjects[preTrainPassObjects.CurrentIndex + 1];

                double newPreTrainLocation;
                if (preTrainPassObjects.CurrentIndex < 0)
                {
                    newPreTrainLocation = preTrainPassObjects[0].Location;
                }
                else if (preTrainPassObjects.CurrentIndex >= preTrainPassObjects.Count - 1)
                {
                    newPreTrainLocation = preTrainPassObjects[preTrainPassObjects.Count - 1].Location;
                }
                else
                {
                    double timePassingRate = (double)(timeMilliseconds - currentObject.Value) / (nextObject.Value - currentObject.Value);
                    newPreTrainLocation = (1.0 - timePassingRate) * currentObject.Location + timePassingRate * nextObject.Location;
                }

                source = PreTrainLocation.FromLocation(newPreTrainLocation, sectionManager);
            }

            PreTrainLocation converted = converter.Convert(source);

            sectionManager.PreTrainLocation = converted.Location;
            sectionManager.PreTrainSectionIndex = converted.SectionIndex;

            if (source.SectionIndex != converted.SectionIndex)
            {
                sectionManager.OnSectionChanged();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            HarmonyPatch.Dispose();
        }
    }
}
