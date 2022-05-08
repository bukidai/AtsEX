using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class TimeManager : ClassWrapper
    {
        static TimeManager()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<TimeManager>();

            GameStateGetMethod = members.GetSourcePropertyGetterOf(nameof(GameState));
            GameStateSetMethod = members.GetSourcePropertySetterOf(nameof(GameState));

            TimeMillisecondsGetMethod = members.GetSourcePropertyGetterOf(nameof(TimeMilliseconds));

            SetTimeMethod = members.GetSourceMethodOf(nameof(SetTime));
        }

        private TimeManager(object src) : base(src)
        {
        }

        public static TimeManager FromSource(object src)
        {
            if (src is null) return null;
            return new TimeManager(src);
        }

        private static MethodInfo GameStateGetMethod;
        private static MethodInfo GameStateSetMethod;
        public GameState GameState
        {
            get => GameStateGetMethod.Invoke(Src, null);
            set => GameStateSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TimeMillisecondsGetMethod;
        public int TimeMilliseconds
        {
            get => TimeMillisecondsGetMethod.Invoke(Src, null);
        }

        public TimeSpan Time => TimeSpan.FromMilliseconds(TimeMilliseconds);

        private static MethodInfo SetTimeMethod;
        public void SetTime(int timeMilliseconds) => SetTimeMethod.Invoke(Src, new object[] { timeMilliseconds });

        public void SetTime(TimeSpan time) => SetTime((int)time.TotalMilliseconds);
    }
}
