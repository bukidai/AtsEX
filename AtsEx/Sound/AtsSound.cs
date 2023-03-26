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

        private Queue<int> CommandQueue = new Queue<int>();

        public PlayState PlayState { get; private set; }

        public AtsSound()
        {
        }

        public int Tick() => CommandQueue.Count == 0 ? (int)SoundPlayType.Continue : CommandQueue.Dequeue();

        public void Play()
        {
            CommandQueue.Enqueue((int)SoundPlayType.Once);
            PlayState = PlayState.Playing;
        }

        public void PlayLoop()
        {
            CommandQueue.Enqueue((int)SoundPlayType.Loop);
            PlayState = PlayState.PlayingLoop;
        }

        public void PlayLoop(double volume)
        {
            int commandValue = (int)(volume * 100);
            if (commandValue < MinVolume || MaxVolume < commandValue)
            {
                throw new ArgumentOutOfRangeException(nameof(volume));
            }

            CommandQueue.Enqueue(commandValue);
            PlayState = PlayState.PlayingLoop;
        }

        public void Stop()
        {
            CommandQueue.Enqueue((int)SoundPlayType.Stop);
            PlayState = PlayState.Stop;
        }

        public void StopAndPlay()
        {
            CommandQueue.Enqueue((int)SoundPlayType.Stop);
            CommandQueue.Enqueue((int)SoundPlayType.Once);
            PlayState = PlayState.Playing;
        }
    }
}
