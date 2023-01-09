using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

namespace ObjectiveHarmonyPatch
{
    internal sealed class HarmonyPatchHost : IDisposable
    {
        private static readonly Harmony Harmony = new Harmony("com.objective-harmony-patch");
        private static readonly Dictionary<MethodBase, HarmonyPatchHost> PatchHosts = new Dictionary<MethodBase, HarmonyPatchHost>();

        private readonly MethodBase Original;
        private readonly List<HarmonyPatch> Prefix = new List<HarmonyPatch>();
        private readonly List<HarmonyPatch> Postfix = new List<HarmonyPatch>();

        private bool IsEmpty => Prefix.Count == 0 && Postfix.Count == 0;

        private HarmonyPatchHost(MethodBase original)
        {
            Original = original;

            bool isStatic = original.IsStatic;
            bool hasReturnValue = Original is MethodInfo method && method.ReturnType != typeof(void);

            Type[] patchMethodArguments = GetHarmonyMethodArgumentTypes();

            HarmonyMethod prefix = new HarmonyMethod(typeof(HarmonyPatchHost), nameof(PrefixMethod), patchMethodArguments);
            HarmonyMethod postfix = new HarmonyMethod(typeof(HarmonyPatchHost), nameof(PostfixMethod), patchMethodArguments);

            _ = Harmony.Patch(Original, prefix, postfix);
            PatchHosts.Add(Original, this);


            Type[] GetHarmonyMethodArgumentTypes()
            {
                List<Type> args = new List<Type>();

                if (!isStatic) args.Add(typeof(object));
                if (hasReturnValue) args.Add(typeof(object).MakeByRefType());
                args.AddRange(new Type[]
                {
                    typeof(object[]),
                    typeof(MethodBase),
                    typeof(bool),
                });

                return args.ToArray();
            }
        }

        private static HarmonyPatchHost GetOrCreate(MethodBase original)
            => PatchHosts.TryGetValue(original, out HarmonyPatchHost result) ? result : new HarmonyPatchHost(original);

        public void Dispose()
        {
            Harmony.Unpatch(Original, HarmonyPatchType.All, Harmony.Id);
            PatchHosts.Remove(Original);
        }

        private static void Patch(HarmonyPatch patch, Func<HarmonyPatchHost, List<HarmonyPatch>> targetSelector)
        {
            HarmonyPatchHost patchHost = GetOrCreate(patch.Original);

            List<HarmonyPatch> patches = targetSelector(patchHost);
            if (patches.Contains(patch)) throw new InvalidOperationException();

            patches.Add(patch);
        }

        public static void PatchPrefix(HarmonyPatch patch) => Patch(patch, patchHost => patchHost.Prefix);
        public static void PatchPostfix(HarmonyPatch patch) => Patch(patch, patchHost => patchHost.Postfix);

        private static void Unpatch(HarmonyPatch patch, Func<HarmonyPatchHost, List<HarmonyPatch>> targetSelector)
        {
            if (!PatchHosts.TryGetValue(patch.Original, out HarmonyPatchHost patchHost)) throw new KeyNotFoundException();

            List<HarmonyPatch> patches = targetSelector(patchHost);
            patches.Remove(patch);

            if (patchHost.IsEmpty) patchHost.Dispose();
        }

        public static void UnpatchPrefix(HarmonyPatch patch) => Unpatch(patch, patchHost => patchHost.Prefix);
        public static void UnpatchPostfix(HarmonyPatch patch) => Unpatch(patch, patchHost => patchHost.Postfix);

#pragma warning disable IDE1006 // 命名スタイル
        #region パッチメソッド
        private static bool PrefixMethod(object __instance, ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(__instance, ref __result, __args, __originalMethod, __runOriginal, patchHost => patchHost.Prefix);

        private static bool PrefixMethod(object __instance, object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            return InvokePatches(__instance, ref _, __args, __originalMethod, __runOriginal, patchHost => patchHost.Prefix);
        }

        private static bool PrefixMethod(ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(null, ref __result, __args, __originalMethod, __runOriginal, patchHost => patchHost.Prefix);

        private static bool PrefixMethod(object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            return InvokePatches(null, ref _, __args, __originalMethod, __runOriginal, patchHost => patchHost.Prefix);
        }

        private static void PostfixMethod(object __instance, ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(__instance, ref __result, __args, __originalMethod, __runOriginal, patchHost => patchHost.Postfix);

        private static void PostfixMethod(object __instance, object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            InvokePatches(__instance, ref _, __args, __originalMethod, __runOriginal, patchHost => patchHost.Postfix);
        }

        private static void PostfixMethod(ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(null, ref __result, __args, __originalMethod, __runOriginal, patchHost => patchHost.Postfix);

        private static void PostfixMethod(object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            InvokePatches(null, ref _, __args, __originalMethod, __runOriginal, patchHost => patchHost.Postfix);
        }
        #endregion

        private static bool InvokePatches(object __instance, ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal,
            Func<HarmonyPatchHost, List<HarmonyPatch>> patchTypeSelector)
        {
            bool skipOriginal = false;

            PatchInvokedEventArgs args;
            HarmonyPatchHost patchHost = PatchHosts[__originalMethod];
            foreach (HarmonyPatch patch in patchTypeSelector(patchHost))
            {
                args = new PatchInvokedEventArgs(__instance, __result, __args, __runOriginal, skipOriginal);

                PatchInvokationResult result = patch.Invoke(patch, args);
                if (result is null) continue;

                if (result.ChangeReturnValue) __result = result.ReturnValue;
                skipOriginal = result.SkipModes.HasFlag(SkipModes.SkipOriginal);

                if (result.SkipModes.HasFlag(SkipModes.SkipPatches)) break;
            }

            return !skipOriginal;
        }
#pragma warning restore IDE1006 // 命名スタイル
    }
}
