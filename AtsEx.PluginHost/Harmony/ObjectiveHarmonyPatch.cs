using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

namespace Automatic9045.AtsEx.PluginHost.Harmony
{
    /// <summary>
    /// オブジェクト (インスタンス) として扱える Harmony パッチのラッパーを提供します。
    /// </summary>
    public sealed partial class ObjectiveHarmonyPatch : IDisposable
    {
        private static readonly HarmonyLib.Harmony Harmony = new HarmonyLib.Harmony("com.automatic9045.atsex.dynamic-harmony-host");
        private static readonly SortedList<MethodBase, List<ObjectiveHarmonyPatch>> Patches = new SortedList<MethodBase, List<ObjectiveHarmonyPatch>>(new MethodComaparer());

        private readonly MethodBase Original;
        private readonly HarmonyMethod PrefixHarmonyMethod;
        private readonly HarmonyMethod PostfixHarmonyMethod;

        /// <summary>
        /// Harmony パッチの Prefix メソッドが実行されたときに発生します。
        /// </summary>
        public event PatchInvokedEventHandler Prefix;

        /// <summary>
        /// Harmony パッチの Postfix メソッドが実行されたときに発生します。
        /// </summary>
        public event PatchInvokedEventHandler Postfix;

        private ObjectiveHarmonyPatch(MethodBase original)
        {
            Original = original;

            bool isStatic = original.IsStatic;
            bool hasReturnValue = Original is MethodInfo method && method.ReturnType != typeof(void);

            Type[] patchMethodArgs = GetValidPatchMethodArgumentList(isStatic, hasReturnValue);

            PrefixHarmonyMethod = new HarmonyMethod(typeof(ObjectiveHarmonyPatch), nameof(PrefixMethod), patchMethodArgs);
            PostfixHarmonyMethod = new HarmonyMethod(typeof(ObjectiveHarmonyPatch), nameof(PostfixMethod), patchMethodArgs);

            _ = Harmony.Patch(Original, PrefixHarmonyMethod, PostfixHarmonyMethod);

            if (!Patches.ContainsKey(Original)) Patches[Original] = new List<ObjectiveHarmonyPatch>();
            Patches[original].Add(this);
        }

        private static Type[] GetValidPatchMethodArgumentList(bool isStatic, bool hasReturnValue)
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

#pragma warning disable IDE1006 // 命名スタイル
        private static bool PrefixMethod(object __instance, ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(__instance, ref __result, __args, __originalMethod, __runOriginal, patch => patch.Prefix);

        private static bool PrefixMethod(object __instance, object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            return InvokePatches(__instance, ref _, __args, __originalMethod, __runOriginal, patch => patch.Prefix);
        }

        private static bool PrefixMethod(ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(null, ref __result, __args, __originalMethod, __runOriginal, patch => patch.Prefix);

        private static bool PrefixMethod(object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            return InvokePatches(null, ref _, __args, __originalMethod, __runOriginal, patch => patch.Prefix);
        }

        private static void PostfixMethod(object __instance, ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(__instance, ref __result, __args, __originalMethod, __runOriginal, patch => patch.Postfix);

        private static void PostfixMethod(object __instance, object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            InvokePatches(__instance, ref _, __args, __originalMethod, __runOriginal, patch => patch.Postfix);
        }

        private static void PostfixMethod(ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal)
            => InvokePatches(null, ref __result, __args, __originalMethod, __runOriginal, patch => patch.Postfix);

        private static void PostfixMethod(object[] __args, MethodBase __originalMethod, bool __runOriginal)
        {
            object _ = null;
            InvokePatches(null, ref _, __args, __originalMethod, __runOriginal, patch => patch.Postfix);
        }

        private static bool InvokePatches(object __instance, ref object __result, object[] __args, MethodBase __originalMethod, bool __runOriginal,
            Func<ObjectiveHarmonyPatch, PatchInvokedEventHandler> eventSelector)
        {
            bool cancel = false;

            PatchInvokedEventArgs e = new PatchInvokedEventArgs(__instance, __result, __args, __runOriginal);
            foreach (ObjectiveHarmonyPatch patch in Patches[__originalMethod])
            {
                PatchInvokationResult result = eventSelector(patch)?.Invoke(patch, e);
                if (result is null) continue;

                if (result.ChangeReturnValue) __result = result.ReturnValue;
                cancel = result.Cancel;
            }

            return !cancel;
        }
#pragma warning restore IDE1006 // 命名スタイル

        /// <summary>
        /// 指定したメソッドに Harmony パッチを適用します。
        /// </summary>
        /// <param name="original">パッチを適用するメソッド。</param>
        /// <returns>パッチを表す <see cref="ObjectiveHarmonyPatch"/>。</returns>
        public static ObjectiveHarmonyPatch Patch(MethodBase original) => new ObjectiveHarmonyPatch(original);

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!(PrefixHarmonyMethod is null)) Harmony.Unpatch(Original, PrefixHarmonyMethod.method);
            if (!(PostfixHarmonyMethod is null)) Harmony.Unpatch(Original, PostfixHarmonyMethod.method);
        }
    }
}
