using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.Helpers
{
    internal class StructureSetLifeProlonger
    {
        static StructureSetLifeProlonger()
        {
            Harmony harmony = new Harmony("com.automatic9045.atsex.classwrappers.structureset-life-prolonger");
            harmony.Patch(cz.OriginalMemberSet.SetRouteMethod, new HarmonyMethod(typeof(StructureSetLifeProlonger), nameof(SetRoutePrefix)));
        }

        private static void SetRoutePrefix(object[] __args)
        {
            Route route = Route.FromSource(__args[0]);
            StructureSet structures = route.Structures;
            InstanceStore.BveHacker.PreviewScenarioCreated += e =>
            {
                e.Scenario.Route.Structures = structures;
            };
        }
    }
}
