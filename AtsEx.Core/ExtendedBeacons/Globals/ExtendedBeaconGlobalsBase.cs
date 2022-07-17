using Automatic9045.AtsEx.PluginHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.Plugins.Scripting;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    public abstract class ExtendedBeaconGlobalsBase<TPassedEventArgs> : Globals where TPassedEventArgs : PassedEventArgs
    {
#pragma warning disable IDE1006 // 命名スタイル
        public readonly PluginHost.ExtendedBeacons.ExtendedBeaconBase<TPassedEventArgs> sender;
        public readonly TPassedEventArgs e;
#pragma warning restore IDE1006 // 命名スタイル

        public ExtendedBeaconGlobalsBase(IBveHacker bveHacker, PluginHost.ExtendedBeacons.ExtendedBeaconBase<TPassedEventArgs> sender, TPassedEventArgs eventArgs) : base(bveHacker)
        {
            this.sender = sender;
            e = eventArgs;
        }
    }
}
