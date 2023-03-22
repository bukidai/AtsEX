using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.Native.Ats;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Sound.Native;

namespace AtsEx.Sound
{
    internal sealed class AtsSoundSet : IAtsSoundSet
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<AtsSoundSet>("Core");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ChangeConflicted { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private const int MinIndex = 0;
        private const int MaxIndex = 255;

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static AtsSoundSet()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private readonly Dictionary<int, AtsSound> RegisteredSounds = new Dictionary<int, AtsSound>();
        private readonly Dictionary<int, int> OldCommandValues = new Dictionary<int, int>();

        private bool IsFirstFrame = true;

        public AtsSoundSet()
        {
        }

        public void Tick(AtsIoArray source)
        {
            foreach (KeyValuePair<int, AtsSound> x in RegisteredSounds)
            {
                if (source[x.Key] != (int)SoundPlayType.Continue && OldCommandValues.TryGetValue(x.Key, out int oldValue) && source[x.Key] != oldValue)
                {
                    string senderName = x.Key.ToString();
                    throw new ConflictException(string.Format(Resources.Value.ChangeConflicted.Value, senderName), senderName);
                }

                int commandValue = IsFirstFrame ? (int)SoundPlayType.Stop : x.Value.Tick();

                source[x.Key] = commandValue;
                OldCommandValues[x.Key] = commandValue;
            }

            IsFirstFrame = false;
        }

        public IAtsSound Register(int index)
        {
            if (index < MinIndex || MaxIndex < index) throw new IndexOutOfRangeException();

            AtsSound atsSound = new AtsSound();

            RegisteredSounds.Add(index, atsSound);
            return atsSound;
        }
    }
}
