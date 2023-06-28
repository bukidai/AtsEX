using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastMember;
using TypeWrapping;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// サウンドを表します。
    /// </summary>
    public class Sound : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<Sound>();

            PlayMethod = members.GetSourceMethodOf(nameof(Play));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="Sound"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected Sound(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Sound"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static Sound FromSource(object src) => src is null ? null : new Sound(src);

        private static FastMethod PlayMethod;
        /// <summary>
        /// 音声を再生します。
        /// </summary>
        /// <param name="volume">音声を再生する音量。</param>
        /// <param name="pitch">音声を再生するピッチ。</param>
        /// <param name="fadeTimeMilliseconds">音量のフェードインにかける時間 [ms]。</param>
        public void Play(double volume, double pitch, int fadeTimeMilliseconds)
            => PlayMethod.Invoke(Src, new object[] { volume, pitch, fadeTimeMilliseconds });
    }
}
