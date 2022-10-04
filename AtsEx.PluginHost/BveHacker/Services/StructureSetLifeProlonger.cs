using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using ObjectiveHarmonyPatch;

using Automatic9045.AtsEx.PluginHost.BveTypes;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.BveHackerServices
{
    internal sealed class StructureSetLifeProlonger : IDisposable
    {
        private readonly HarmonyPatch SetRouteMethodPatch;

        private readonly BveHacker BveHacker;

        public StructureSetLifeProlonger(BveHacker bveHacker)
        {
            BveHacker = bveHacker;

            ClassMemberSet trainDrawerMembers = BveHacker.BveTypes.GetClassInfoOf<ObjectDrawer>();
            FastMethod setRouteMethod = trainDrawerMembers.GetSourceMethodOf(nameof(ObjectDrawer.SetRoute));

            SetRouteMethodPatch = HarmonyPatch.Patch(setRouteMethod.Source);
            SetRouteMethodPatch.Prefix += (_, e) =>
            {
                Route route = Route.FromSource(e.Args[0]);
                StructureSet structures = route.Structures;

                BveHacker.PreviewScenarioCreated += e2 => e2.Scenario.Route.Structures = structures;

                return new PatchInvokationResult();
            };
        }

        public void Dispose()
        {
            SetRouteMethodPatch.Dispose();
        }
    }
}
