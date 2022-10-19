using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.Plugins.Scripting;
using AtsEx.PluginHost.ClassWrappers;
using AtsEx.PluginHost.ExtendedBeacons;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.ExtendedBeacons
{
    internal abstract class ExtendedBeaconBase<TPassedEventArgs> : PluginHost.ExtendedBeacons.ExtendedBeaconBase<TPassedEventArgs>, ICompilationErrorCheckable
        where TPassedEventArgs : PassedEventArgs
    {
        protected readonly BveHacker BveHacker;
        protected IReadOnlyDictionary<PluginType, PluginVariableCollection> PluginVariables;

        protected readonly IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> Script;

        public ExtendedBeaconBase(BveHacker bveHacker, IReadOnlyDictionary<PluginType, PluginVariableCollection> pluginVariables,
            string name, RepeatedStructure definedStructure, ObservingTargetTrack observingTargetTrack, ObservingTargetTrain observingTargetTrain,
            IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> script)
            : base(name, definedStructure, observingTargetTrack, observingTargetTrain)
        {
            BveHacker = bveHacker;
            PluginVariables = pluginVariables;

            Script = script;
        }

        public void CheckCompilationErrors() => _ = Script.GetWithCheckErrors();
    }
}
