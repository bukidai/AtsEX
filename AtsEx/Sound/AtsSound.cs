using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Native;
using AtsEx.PluginHost.Sound;
using AtsEx.PluginHost.Sound.Native;

namespace AtsEx.Sound
{
    internal sealed class AtsSound : IAtsSound
    {
        private const int MinVolume = -9999;
        private const int MaxVolume = 0;

        private int CommandValue = (int)SoundPlayType.Continue;

        public PlayState PlayState { get; private set; }

        public AtsSound()
        {
        }

        public int Tick()
        {
            int result = CommandValue;
            CommandValue = (int)SoundPlayType.Continue;

            return result;
        }

        public void Play()
        {
            CommandValue = (int)SoundPlayType.Once;
            PlayState = PlayState.Playing;
        }

        public void PlayLoop()
        {
            CommandValue = (int)SoundPlayType.Loop;
            PlayState = PlayState.PlayingLoop;
        }

        public void PlayLoop(double volume)
        {
            int commandValue = (int)(volume * 100);
            if (commandValue < MinVolume || MaxVolume < commandValue)
            {
                throw new ArgumentOutOfRangeException(nameof(volume));
            }

            CommandValue = commandValue;
            PlayState = PlayState.PlayingLoop;
        }

        public void Stop()
        {
            CommandValue = (int)SoundPlayType.Stop;
            PlayState = PlayState.Stop;
        }
    }
}
