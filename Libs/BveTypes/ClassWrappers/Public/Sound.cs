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

            PlayMethod1 = members.GetSourceMethodOf(nameof(Play), new Type[] { typeof(double), typeof(double), typeof(int) });
            PlayMethod2 = members.GetSourceMethodOf(nameof(Play), new Type[] { typeof(double), typeof(double), typeof(int), typeof(int) });
            PlayLoopingMethod = members.GetSourceMethodOf(nameof(PlayLooping));
            StopMethod = members.GetSourceMethodOf(nameof(Stop));
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

        private static FastMethod PlayMethod1;
        /// <summary>
        /// 音声を冒頭から再生します。
        /// </summary>
        /// <param name="volume">音声を再生する音量。</param>
        /// <param name="pitch">音声を再生するピッチ。</param>
        /// <param name="fadeTimeMilliseconds">音量のフェードインにかける時間 [ms]。</param>
        public void Play(double volume, double pitch, int fadeTimeMilliseconds)
            => PlayMethod1.Invoke(Src, new object[] { volume, pitch, fadeTimeMilliseconds });

        private static FastMethod PlayMethod2;
        /// <summary>
        /// 音声を指定した位置から再生します。
        /// </summary>
        /// <param name="volume">音声を再生する音量。</param>
        /// <param name="pitch">音声を再生するピッチ。</param>
        /// <param name="fadeTimeMilliseconds">音量のフェードインにかける時間 [ms]。</param>
        /// <param name="playPositionBytes">音声の再生を開始する位置 [bytes]。</param>
        public void Play(double volume, double pitch, int fadeTimeMilliseconds, int playPositionBytes)
            => PlayMethod2.Invoke(Src, new object[] { volume, pitch, fadeTimeMilliseconds, playPositionBytes });

        private static FastMethod PlayLoopingMethod;
        /// <summary>
        /// 音声をループ再生します。
        /// </summary>
        /// <param name="volume">音声を再生する音量。</param>
        /// <param name="pitch">音声を再生するピッチ。</param>
        /// <param name="fadeTimeMilliseconds">音量のフェードインにかける時間 [ms]。</param>
        /// <param name="playPositionBytes">音声の再生を開始する位置 [bytes]。</param>
        public void PlayLooping(double volume, double pitch, int fadeTimeMilliseconds, int playPositionBytes)
            => PlayLoopingMethod.Invoke(Src, new object[] { volume, pitch, fadeTimeMilliseconds, playPositionBytes });

        private static FastMethod StopMethod;
        /// <summary>
        /// 音声の再生を停止します。
        /// </summary>
        /// <param name="fadeTimeMilliseconds">音量のフェードアウトにかける時間 [ms]。</param>
        public void Stop(int fadeTimeMilliseconds)
            => StopMethod.Invoke(Src, new object[] { fadeTimeMilliseconds });
    }
}
