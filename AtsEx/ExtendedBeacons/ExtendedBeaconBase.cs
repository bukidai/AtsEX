using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost.ExtendedBeacons;
using AtsEx.Scripting;

namespace AtsEx.ExtendedBeacons
{
    internal abstract class ExtendedBeaconBase<TPassedEventArgs> : PluginHost.ExtendedBeacons.ExtendedBeaconBase<TPassedEventArgs>, ICompilationErrorCheckable
        where TPassedEventArgs : PassedEventArgs
    {
        protected readonly NativeImpl Native;
        protected readonly BveHacker BveHacker;

        protected readonly IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> Script;

        public ExtendedBeaconBase(NativeImpl native, BveHacker bveHacker,
            string name, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, ObservingTargetTrain observingTargetTrain,
            IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> script)
            : base(name, definedStructure, observingTargetTrack, observingTargetTrain)
        {
            Native = native;
            BveHacker = bveHacker;

            Script = script;
        }

        public void CheckCompilationErrors() => _ = Script.GetWithCheckErrors();
    }
}
