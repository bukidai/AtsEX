using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Input
{
    /// <summary>
    /// キーの状態を表します。
    /// このクラスはスレッド セーフです。
    /// </summary>
    public class KeyState : IPressableKey
    {
        /// <summary>
        /// キーが押されているかを取得します。
        /// </summary>
        public bool IsPressed { get; protected set; }

        /// <summary>
        /// キーが押されてから経過した時間 [ms] を取得します。
        /// キーが押されていない場合は 0 を返します。
        /// </summary>
        public long ElapsedMillisecondsSincePressed => IsPressed ? Stopwatch.ElapsedMilliseconds : 0;
        /// <summary>
        /// キーが押されてから経過した時間を取得します。
        /// キーが押されていない場合は <see cref="TimeSpan.Zero"/> を返します。
        /// </summary>
        public TimeSpan ElapsedTimeSincePressed => IsPressed ? Stopwatch.Elapsed : TimeSpan.Zero;

        /// <summary>
        /// キーが押された瞬間に発生します。
        /// </summary>
        public event EventHandler Pressed;
        /// <summary>
        /// キーが離され、<see cref="IsPressed"/> に <see langword="false"/> が設定される直前に発生します。
        /// </summary>
        /// <remarks>
        /// このイベントが発生した時点で経過時間を取得するための <see cref="System.Diagnostics.Stopwatch"/> は停止しているため、<br/>
        /// <see cref="ElapsedMillisecondsSincePressed"/> プロパティ、<see cref="ElapsedTimeSincePressed"/> プロパティから取得できる値は常に不変です。
        /// </remarks>
        /// <seealso cref="Released"/>
        public event EventHandler PreviewReleased;
        /// <summary>
        /// キーが離された瞬間に発生します。
        /// </summary>
        /// <remarks>
        /// キーが押されてから離されるまでに経過した時間を取得するには、<see cref="PreviewReleased"/> イベントを使用してください。
        /// </remarks>
        /// <seealso cref="PreviewReleased"/>
        public event EventHandler Released;

        /// <summary>
        /// キーが押されてから経過した時間を提供するための <see cref="System.Diagnostics.Stopwatch"/> です。
        /// </summary>
        protected readonly Stopwatch Stopwatch = new Stopwatch();

        /// <summary>
        /// スレッド セーフを保証するためのロック オブジェクトです。
        /// </summary>
        protected readonly object LockObj = new object();

        /// <summary>
        /// <see cref="KeyState"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public KeyState()
        {
        }

        /// <summary>
        /// この <see cref="KeyState"/> オブジェクトに対するロックを取得し、指定したデリゲートを実行します。<br/>
        /// <see cref="Press"/>、<see cref="Release"/> を含むデリゲートを指定するとデッドロックするので注意してください。
        /// </summary>
        /// <param name="action">実行する <see cref="Action"/> デリゲート。</param>
        public void LockAndInvoke(Action action)
        {
            lock (LockObj) action();
        }

        void IPressableKey.Press()
        {
            lock (LockObj)
            {
                if (IsPressed) return;

                IsPressed = true;
                Stopwatch.Restart();
                Pressed?.Invoke(this, EventArgs.Empty);
            }
        }

        void IPressableKey.Release()
        {
            lock (LockObj)
            {
                if (!IsPressed) return;

                Stopwatch.Stop();
                PreviewReleased?.Invoke(this, EventArgs.Empty);
                IsPressed = false;
                Released?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
