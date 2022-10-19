using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Mackoy.Bvets;

using FastMember;
using TypeWrapping;

using AtsEx.PluginHost.BveTypes;

namespace AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// キー入力を管理します。
    /// </summary>
    public class KeyProvider : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<KeyProvider>();

            InputDevicesGetMethod = members.GetSourcePropertyGetterOf(nameof(InputDevices));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="KeyProvider"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected KeyProvider(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="KeyProvider"/> クラスのインスタンス。</returns>
        public static KeyProvider FromSource(object src) => src is null ? null : new KeyProvider(src);

        private static FastMethod InputDevicesGetMethod;
        /// <summary>
        /// 読み込まれている入力デバイスプラグインを取得します。
        /// </summary>
        public Dictionary<string, IInputDevice> InputDevices => InputDevicesGetMethod.Invoke(Src, null);
    }
}
