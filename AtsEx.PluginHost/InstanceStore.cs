using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Helpers;

namespace Automatic9045.AtsEx.PluginHost
{
    internal delegate void InitializedEventHandler(EventArgs e);
    internal delegate void ClosingEventHandler(EventArgs e);

    public static class InstanceStore
    {
        public static bool IsInitialized { get; private set; }

        private static IApp app;
        internal static IApp App => IsInitialized ? app : throw new InvalidOperationException($"{nameof(InstanceStore)} は初期化されていません。");

        private static IBveHacker bveHacker;
        internal static IBveHacker BveHacker => IsInitialized ? bveHacker : throw new InvalidOperationException($"{nameof(InstanceStore)} は初期化されていません。");

        internal static event InitializedEventHandler Initialized;
        internal static event ClosingEventHandler Closing;

        public static void Initialize(IApp app, IBveHacker bveHacker)
        {
            if (IsInitialized) throw new InvalidOperationException($"{nameof(InstanceStore)} は既に初期化されています。");
            IsInitialized = true;

            InstanceStore.app = app;
            InstanceStore.bveHacker = bveHacker;

            StaticConstructorInvoker.InvokeAll();

            Initialized?.Invoke(EventArgs.Empty);
        }

        public static void Dispose()
        {
            Closing?.Invoke(EventArgs.Empty);

            app = null;
            bveHacker = null;
            Initialized = null;
            Closing = null;
            IsInitialized = false;
        }
    }
}
