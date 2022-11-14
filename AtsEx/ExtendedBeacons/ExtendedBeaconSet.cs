using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Scripting.Hosting;

using BveTypes.ClassWrappers;
using UnembeddedResources;

using AtsEx.Plugins.Scripting;
using AtsEx.PluginHost;
using AtsEx.PluginHost.ExtendedBeacons;
using AtsEx.Scripting;
using AtsEx.Scripting.IronPython2;

using Identifiers = AtsEx.MapStatementIdentifiers;
using ExtendedBeaconIdentifiers = AtsEx.ExtendedBeacons.MapStatementIdentifiers;
using ScriptIdentifiers = AtsEx.Plugins.Scripting.MapStatementIdentifiers;

using BeaconBase = AtsEx.PluginHost.ExtendedBeacons.ExtendedBeaconBase<AtsEx.PluginHost.ExtendedBeacons.PassedEventArgs>;
using TrainObservingBeaconBase = AtsEx.PluginHost.ExtendedBeacons.ExtendedBeaconBase<AtsEx.PluginHost.ExtendedBeacons.TrainPassedEventArgs>;

namespace AtsEx.ExtendedBeacons
{
    internal partial class ExtendedBeaconSet : PluginHost.ExtendedBeacons.ExtendedBeaconSet
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<ExtendedBeaconSet>(@"Core\ExtendedBeacons");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> BeaconRepeaterNotEnded { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ArgumentMissing { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ItemName { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> ScriptLanguageNotRecognized { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static ExtendedBeaconSet()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        public override ReadOnlyDictionary<string, BeaconBase> Beacons { get; }
        public override ReadOnlyDictionary<string, TrainObservingBeaconBase> TrainObservingBeacons { get; }
        public override ReadOnlyDictionary<string, BeaconBase> PreTrainObservingBeacons { get; }

        protected ExtendedBeaconSet(
            IDictionary<string, BeaconBase> beacons, IDictionary<string, TrainObservingBeaconBase> trainObservingBeacons, IDictionary<string, BeaconBase> preTrainObservingBeacons)
        {
            Beacons = new ReadOnlyDictionary<string, BeaconBase>(beacons);
            TrainObservingBeacons = new ReadOnlyDictionary<string, TrainObservingBeaconBase>(trainObservingBeacons);
            PreTrainObservingBeacons = new ReadOnlyDictionary<string, BeaconBase>(preTrainObservingBeacons);
        }

        public static ExtendedBeaconSet Load(NativeImpl native, BveHacker bveHacker, IDictionary<string, MapObjectList> repeatedStructures, IDictionary<string, Model> structureModels, IDictionary<string, Train> trains)
        {
            Dictionary<string, BeaconBase> beacons = new Dictionary<string, BeaconBase>();
            Dictionary<string, TrainObservingBeaconBase> trainObservingBeacons = new Dictionary<string, TrainObservingBeaconBase>();
            Dictionary<string, BeaconBase> preTrainObservingBeacons = new Dictionary<string, BeaconBase>();

            List<ICompilationErrorCheckable> errorCheckList = new List<ICompilationErrorCheckable>();

            Dictionary<Type, Dictionary<ScriptLanguage, Dictionary<string, dynamic>>> cachedScripts = new Dictionary<Type, Dictionary<ScriptLanguage, Dictionary<string, dynamic>>>();

            foreach (KeyValuePair<string, MapObjectList> sameKeyRepeaters in repeatedStructures)
            {
                string repeaterKey = sameKeyRepeaters.Key;

                (RepeatedStructure Structure, bool IsBeacon) previous = default;
                bool isFirst = true;
                foreach (RepeatedStructure repeatedStructure in sameKeyRepeaters.Value)
                {
                    if (!isFirst && previous.IsBeacon)
                    {
                        if (!(repeatedStructure.Models is null) || repeatedStructure.Location != previous.Structure.Location)
                        {
                            bveHacker.LoadErrorManager.Throw(Resources.Value.BeaconRepeaterNotEnded.Value);
                        }
                    }

                    previous = (repeatedStructure, false);
                    isFirst = false;

                    if (repeatedStructure.Models is null) continue;
                    if (!repeatedStructure.Models.Any()) continue;

                    {
                        string definerText = structureModels.TryGetKey(repeatedStructure.Models[0]);
                        if (definerText != ExtendedBeaconIdentifiers.Definer) continue;

                        previous.IsBeacon = true;
                    }

                    if (repeatedStructure.Models.Count <= 4) throw new BveFileLoadException(Resources.Value.ArgumentMissing.Value, Resources.Value.ItemName.Value);

                    string name = structureModels.TryGetKey(repeatedStructure.Models[1]);
                    if (name == Identifiers.Null) name = Guid.NewGuid().ToString();

                    string scriptLanguageText = structureModels.TryGetKey(repeatedStructure.Models[2]);
                    if (!ScriptIdentifiers.ScriptLanguages.TryGetKey(scriptLanguageText, out ScriptLanguage scriptLanguage))
                    {
                        throw new BveFileLoadException(ScriptIdentifiers.ErrorTexts.InvalidScriptLanguage(scriptLanguageText), Resources.Value.ItemName.Value);
                    }

                    string trackObservingTypeText = structureModels.TryGetKey(repeatedStructure.Models[3]);
                    if (!ExtendedBeaconIdentifiers.ObservingTargetTracks.TryGetKey(trackObservingTypeText, out ObservingTargetTrack observingTargetTrack))
                    {
                        throw new BveFileLoadException(ExtendedBeaconIdentifiers.ErrorTexts.InvalidObservingTargetTrack(trackObservingTypeText), Resources.Value.ItemName.Value);
                    }

                    for (int i = 4; i < repeatedStructure.Models.Count; i++)
                    {
                        string observeTargetText = structureModels.TryGetKey(repeatedStructure.Models[i]);
                        if (!ExtendedBeaconIdentifiers.ObservingTargetTrains.TryGetKey(observeTargetText, out ObservingTargetTrain observingTargetTrain))
                        {
                            throw new BveFileLoadException(ExtendedBeaconIdentifiers.ErrorTexts.InvalidObservingTargetTrain(observeTargetText), Resources.Value.ItemName.Value);
                        }

                        string code = Regex.Unescape(repeaterKey).Replace('`', '\"');

                        switch (observingTargetTrain)
                        {
                            case ObservingTargetTrain.Myself:
                            {
                                IPluginScript<ExtendedBeaconGlobalsBase<PassedEventArgs>> script = CreateScript<PassedEventArgs>(code, scriptLanguage);
                                Beacon beacon = repeatedStructure.Location < 0
                                    ? new InitializerBeacon(native, bveHacker, name, repeatedStructure, observingTargetTrack, observingTargetTrain, script)
                                    : new Beacon(native, bveHacker, name, repeatedStructure, observingTargetTrack, observingTargetTrain, script);

                                beacons[name] = beacon;
                                errorCheckList.Add(beacon);
                                break;
                            }

                            case ObservingTargetTrain.Trains:
                            {
                                IPluginScript<ExtendedBeaconGlobalsBase<TrainPassedEventArgs>> script = CreateScript<TrainPassedEventArgs>(code, scriptLanguage);
                                TrainObservingBeacon beacon = new TrainObservingBeacon(native, bveHacker, name, repeatedStructure, observingTargetTrack, trains, script);

                                trainObservingBeacons[name] = beacon;
                                errorCheckList.Add(beacon);
                                break;
                            }

                            case ObservingTargetTrain.PreTrain:
                            {
                                IPluginScript<ExtendedBeaconGlobalsBase<PassedEventArgs>> script = CreateScript<PassedEventArgs>(code, scriptLanguage);
                                Beacon beacon = new Beacon(native, bveHacker, name, repeatedStructure, observingTargetTrack, observingTargetTrain, script);

                                preTrainObservingBeacons[name] = beacon;
                                errorCheckList.Add(beacon);
                                break;
                            }
                        }
                    }
                }
            }

            errorCheckList.ForEach(beacon => beacon.CheckCompilationErrors());

            return new ExtendedBeaconSet(OrderBeacons(beacons), OrderBeacons(trainObservingBeacons), OrderBeacons(preTrainObservingBeacons));


            IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> CreateScript<TPassedEventArgs>(string code, ScriptLanguage language) where TPassedEventArgs : PassedEventArgs
            {
                string mainMapDirectory = Path.GetDirectoryName(bveHacker.ScenarioInfo.RouteFiles.SelectedFile.Path);

                if (!cachedScripts.ContainsKey(typeof(TPassedEventArgs))) cachedScripts[typeof(TPassedEventArgs)] = new Dictionary<ScriptLanguage, Dictionary<string, dynamic>>();
                if (!cachedScripts[typeof(TPassedEventArgs)].ContainsKey(language)) cachedScripts[typeof(TPassedEventArgs)][language] = new Dictionary<string, dynamic>();

                if (cachedScripts[typeof(TPassedEventArgs)][language].ContainsKey(code))
                {
                    IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> cachedScript = cachedScripts[typeof(TPassedEventArgs)][language][code] as IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>>;
                    return cachedScript.Clone() as IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>>;
                }

                IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> script;
                switch (language)
                {
                    case ScriptLanguage.CSharpScript:
                        script = new Scripting.CSharp.PluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>>(code, mainMapDirectory, Resources.Value.ItemName.Value);
                        break;

                    case ScriptLanguage.IronPython2:
                        ScriptEngine scriptEngine = ScriptEngineProvider.CreateEngine(mainMapDirectory);
                        ScriptScope scriptScope = ScriptEngineProvider.CreateScope(scriptEngine);

                        script = new Scripting.IronPython2.PluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>>(code, scriptEngine, scriptScope, Resources.Value.ItemName.Value);
                        break;

                    default:
                        throw new DevelopException(string.Format(Resources.Value.ScriptLanguageNotRecognized.Value, language));
                }

                cachedScripts[typeof(TPassedEventArgs)][language][code] = script;

                return script;
            }

            Dictionary<string, TBeacon> OrderBeacons<TBeacon>(Dictionary<string, TBeacon> source) where TBeacon : IExtendedBeacon
                => source.OrderBy(item => item.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public void Tick(double location, double preTrainLocation)
        {
            foreach (Beacon beacon in Beacons.Values) beacon.Tick(location);
            foreach (TrainObservingBeacon beacon in TrainObservingBeacons.Values) beacon.Tick();
            foreach (Beacon beacon in PreTrainObservingBeacons.Values) beacon.Tick(preTrainLocation);
        }
    }
}
