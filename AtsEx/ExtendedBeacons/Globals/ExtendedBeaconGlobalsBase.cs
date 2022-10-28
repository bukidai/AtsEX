using AtsEx.PluginHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Scripting;
using AtsEx.PluginHost.ExtendedBeacons;

namespace AtsEx.ExtendedBeacons
{
    public abstract class ExtendedBeaconGlobalsBase<TPassedEventArgs> : Globals where TPassedEventArgs : PassedEventArgs
    {
#pragma warning disable IDE1006 // 命名スタイル
        public readonly PluginHost.ExtendedBeacons.ExtendedBeaconBase<TPassedEventArgs> sender;
        public readonly TPassedEventArgs e;
#pragma warning restore IDE1006 // 命名スタイル

        public ExtendedBeaconGlobalsBase(INative native, PluginHost.BveHacker bveHacker,
            PluginHost.ExtendedBeacons.ExtendedBeaconBase<TPassedEventArgs> sender, TPassedEventArgs eventArgs)
            : base(native, bveHacker)
        {
            this.sender = sender;
            e = eventArgs;
        }

        internal abstract TPassedEventArgs GetEventArgsWithScriptVariables();
    }
}
