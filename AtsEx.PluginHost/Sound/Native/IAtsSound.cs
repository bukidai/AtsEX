using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.Sound.Native
{
    /// <summary>
    /// ATS プラグインから制御可能な ATS サウンドを表します。
    /// </summary>
    public interface IAtsSound : ISound
    {
        /// <summary>
        /// 音源の本来の音量よりも小さい音量でサウンドをループ再生します。
        /// </summary>
        /// <param name="volume">下げる音量の符号付き大きさ [dB]。0 または負の値で指定してください。</param>
        void PlayLoop(double volume);
    }
}
