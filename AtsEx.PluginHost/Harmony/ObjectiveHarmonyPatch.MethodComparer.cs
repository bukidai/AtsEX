using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Harmony
{
    public sealed partial class ObjectiveHarmonyPatch
    {
        private class MethodComaparer : IComparer<MethodBase>
        {
            public int Compare(MethodBase x, MethodBase y) => x.ToString().CompareTo(y.ToString());
        }
    }
}
