using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.BveHackerServices
{
    internal sealed class StructureSetLifeProlonger : IDisposable
    {
        private static event EventHandler<PatchInvokedEventArgs> PatchInvoked;

        private readonly BveHacker BveHacker;
        private readonly Harmony Harmony = new Harmony("com.automatic9045.atsex.classwrappers.structureset-life-prolonger");

        public StructureSetLifeProlonger(BveHacker bveHacker)
        {
            BveHacker = bveHacker;
            Harmony.Patch(TrainDrawer.OriginalMemberSet.SetRouteMethod, new HarmonyMethod(typeof(StructureSetLifeProlonger), nameof(SetRoutePrefix)));

            PatchInvoked += (_, e) =>
            {
                StructureSet structures = e.Route.Structures;
                BveHacker.PreviewScenarioCreated += e2 => e2.Scenario.Route.Structures = structures;
            };
        }

        public void Dispose()
        {
            Harmony.UnpatchAll();
        }

#pragma warning disable IDE1006 // 命名スタイル
        private static void SetRoutePrefix(object[] __args)
#pragma warning restore IDE1006 // 命名スタイル
        {
            Route route = Route.FromSource(__args[0]);
            PatchInvoked?.Invoke(null, new PatchInvokedEventArgs(route));
        }


        private class PatchInvokedEventArgs : EventArgs
        {
            public Route Route { get; }

            public PatchInvokedEventArgs(Route route)
            {
                Route = route;
            }
        }
    }
}
