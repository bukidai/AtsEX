using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveHarmonyPatch
{
    public sealed partial class HarmonyPatch
    {
        private class MethodComaparer : IComparer<MethodBase>
        {
            public int Compare(MethodBase x, MethodBase y) => x.ToString().CompareTo(y.ToString());
        }
    }
}
