using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveHarmonyPatch
{
    /// <summary>
    /// オブジェクト (インスタンス) として扱える Harmony パッチのラッパーを提供します。
    /// </summary>
    public sealed class HarmonyPatch : IDisposable
    {
        /// <summary>
        /// パッチの名前を取得します。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// このパッチが適用されているメソッドを取得します。
        /// </summary>
        public MethodBase Original { get; }

        /// <summary>
        /// パッチの種類を取得します。
        /// </summary>
        public PatchType PatchType { get; }

        /// <summary>
        /// Harmony パッチが実行されたときに発生します。
        /// </summary>
        public event PatchInvokedEventHandler Invoked;

        private HarmonyPatch(string name, MethodBase original, PatchType patchType)
        {
            Name = name;
            Original = original;
            PatchType = patchType;
        }

        /// <summary>
        /// 指定したメソッドに Harmony パッチを適用します。
        /// </summary>
        /// <param name="name">パッチの名前。</param>
        /// <param name="original">パッチを適用するメソッド。</param>
        /// <param name="patchType">パッチの種類。</param>
        /// <returns>パッチを表す <see cref="HarmonyPatch"/>。</returns>
        public static HarmonyPatch Patch(string name, MethodBase original, PatchType patchType)
        {
            HarmonyPatch patch = new HarmonyPatch(name, original, patchType);
            switch (patchType)
            {
                case PatchType.Prefix:
                    HarmonyPatchHost.PatchPrefix(patch);
                    break;

                case PatchType.Postfix:
                    HarmonyPatchHost.PatchPostfix(patch);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return patch;
        }

        /// <inheritdoc/>
        public override string ToString() => Name ?? base.ToString();

        internal PatchInvokationResult Invoke(object sender, PatchInvokedEventArgs e) => Invoked?.Invoke(sender, e);

        /// <inheritdoc/>
        public void Dispose()
        {
            switch (PatchType)
            {
                case PatchType.Prefix:
                    HarmonyPatchHost.UnpatchPrefix(this);
                    break;

                case PatchType.Postfix:
                    HarmonyPatchHost.UnpatchPostfix(this);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
