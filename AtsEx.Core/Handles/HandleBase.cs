using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Handles;

namespace Automatic9045.AtsEx.Handles
{
    internal abstract class HandleBase : IHandle
    {
        public int MinNotch { get; }
        public int MaxNotch { get; }

        private int _Notch;
        public int Notch
        {
            get => _Notch;
            set
            {
                _Notch = value < MinNotch ? throw new ArgumentOutOfRangeException(nameof(value))
                    : MaxNotch < value ? throw new ArgumentOutOfRangeException(nameof(value))
                    : value;
            }
        }

        /// <summary>
        /// <see cref="HandleBase"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="minNotch">最小のノッチ。</param>
        /// <param name="maxNotch">最大のノッチ。</param>
        public HandleBase(int minNotch, int maxNotch)
        {
            MinNotch = minNotch;
            MaxNotch = maxNotch;
        }

        public NotchCommandBase GetCommandToSetNotchTo(int notch)
        {
            return notch < MinNotch ? throw new ArgumentOutOfRangeException(nameof(notch))
                : MaxNotch < notch ? throw new ArgumentOutOfRangeException(nameof(notch))
                : new NotchCommandBase.SetNotchCommand(notch);
        }

        public int ExecuteNotchCommands(IReadOnlyList<NotchCommandBase> commandEntries)
        {
            foreach (NotchCommandBase command in commandEntries)
            {
                int? newNotch = command.GetOverridenNotch(Notch);
                if (!(newNotch is null)) return (int)newNotch;
            }

            return Notch;
        }
    }
}
