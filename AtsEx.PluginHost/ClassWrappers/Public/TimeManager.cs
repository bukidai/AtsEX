using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 時間に関する処理を行います。
    /// </summary>
    public sealed class TimeManager : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberSet members = BveTypeSet.Instance.GetClassInfoOf<TimeManager>();

            GameStateGetMethod = members.GetSourcePropertyGetterOf(nameof(GameState));
            GameStateSetMethod = members.GetSourcePropertySetterOf(nameof(GameState));

            TimeMillisecondsGetMethod = members.GetSourcePropertyGetterOf(nameof(TimeMilliseconds));

            SetTimeMethod = members.GetSourceMethodOf(nameof(SetTime));
        }

        private TimeManager(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="TimeManager"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static TimeManager FromSource(object src)
        {
            if (src is null) return null;
            return new TimeManager(src);
        }

        private static MethodInfo GameStateGetMethod;
        private static MethodInfo GameStateSetMethod;
        /// <summary>
        /// 時間進行の設定を取得・設定します。
        /// </summary>
        public GameState GameState
        {
            get => GameStateGetMethod.Invoke(Src, null);
            set => GameStateSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TimeMillisecondsGetMethod;
        /// <summary>
        /// 0 時丁度から現在までに経過した時間をミリ秒単位で取得します。
        /// </summary>
        /// <remarks>
        /// 現在時刻の変更には <see cref="SetTime(int)"/> メソッド、<see cref="SetTime(TimeSpan)"/> メソッドを使用してください。
        /// </remarks>
        /// <seealso cref="Time"/>
        /// <seealso cref="SetTime(int)"/>
        /// <seealso cref="SetTime(TimeSpan)"/>
        public int TimeMilliseconds
        {
            get => TimeMillisecondsGetMethod.Invoke(Src, null);
        }

        /// <summary>
        /// 0 時丁度から現在までに経過した時間を表す <see cref="TimeSpan"/> を取得します。
        /// </summary>
        /// <remarks>
        /// 現在時刻の変更には <see cref="SetTime(int)"/> メソッド、<see cref="SetTime(TimeSpan)"/> メソッドを使用してください。
        /// </remarks>
        /// <seealso cref="TimeMilliseconds"/>
        /// <seealso cref="SetTime(int)"/>
        /// <seealso cref="SetTime(TimeSpan)"/>
        public TimeSpan Time => TimeSpan.FromMilliseconds(TimeMilliseconds);

        private static MethodInfo SetTimeMethod;
        /// <summary>
        /// 現在時刻を変更します。
        /// </summary>
        /// <param name="timeMilliseconds">0 時丁度から現在までに経過したミリ秒数 [ms]。</param>
        /// <remarks>
        /// 現在時刻の取得には <see cref="TimeMilliseconds"/> プロパティ、<see cref="Time"/> プロパティを使用してください。
        /// </remarks>
        /// <seealso cref="TimeMilliseconds"/>
        /// <seealso cref="Time"/>
        public void SetTime(int timeMilliseconds) => SetTimeMethod.Invoke(Src, new object[] { timeMilliseconds });

        /// <summary>
        /// 現在時刻を変更します。
        /// </summary>
        /// <param name="time">0 時丁度から現在までに経過した時間。</param>
        /// <remarks>
        /// 現在時刻の取得には <see cref="TimeMilliseconds"/> プロパティ、<see cref="Time"/> プロパティを使用してください。
        /// </remarks>
        /// <see cref="TimeMilliseconds"/>
        /// <see cref="Time"/>
        public void SetTime(TimeSpan time) => SetTime((int)time.TotalMilliseconds);
    }
}
