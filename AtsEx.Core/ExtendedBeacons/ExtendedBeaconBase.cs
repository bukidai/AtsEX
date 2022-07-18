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
    internal abstract class ExtendedBeaconBase<TPassedEventArgs> : PluginHost.ExtendedBeacons.ExtendedBeaconBase<TPassedEventArgs> where TPassedEventArgs : PassedEventArgs
    {
        protected readonly BveHacker BveHacker;
        protected readonly IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> Script;

        public ExtendedBeaconBase(BveHacker bveHacker, string name, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, ObservingTargetTrain observingTargetTrain, IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> script) 
            : base(name, definedStructure, observingTargetTrack, observingTargetTrain)
        {
            BveHacker = bveHacker;
            Script = script;
        }
    }
}
