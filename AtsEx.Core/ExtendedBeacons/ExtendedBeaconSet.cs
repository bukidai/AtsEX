using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Scripting.Hosting;

using Automatic9045.AtsEx.Plugins.Scripting;
using Automatic9045.AtsEx.Plugins.Scripting.IronPython2;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;
using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;
using Automatic9045.AtsEx.PluginHost.Resources;

using Identifiers = Automatic9045.AtsEx.ExtendedBeacons.MapStatementIdentifiers;
using ScriptIdentifiers = Automatic9045.AtsEx.Plugins.Scripting.MapStatementIdentifiers;

using BeaconBase = Automatic9045.AtsEx.PluginHost.ExtendedBeacons.ExtendedBeaconBase<Automatic9045.AtsEx.PluginHost.ExtendedBeacons.PassedEventArgs>;
using TrainObservingBeaconBase = Automatic9045.AtsEx.PluginHost.ExtendedBeacons.ExtendedBeaconBase<Automatic9045.AtsEx.PluginHost.ExtendedBeacons.TrainPassedEventArgs>;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal partial class ExtendedBeaconSet : IExtendedBeaconSet
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<ExtendedBeaconSet>(@"Core\ExtendedBeacons");

        public ReadOnlyDictionary<string, BeaconBase> Beacons { get; }
        public ReadOnlyDictionary<string, TrainObservingBeaconBase> TrainObservingBeacons { get; }
        public ReadOnlyDictionary<string, BeaconBase> PreTrainObservingBeacons { get; }

        protected ExtendedBeaconSet(IDictionary<string, BeaconBase> beacons, IDictionary<string, TrainObservingBeaconBase> trainObservingBeacons, IDictionary<string, BeaconBase> preTrainObservingBeacons)
        {
            Beacons = new ReadOnlyDictionary<string, BeaconBase>(beacons);
            TrainObservingBeacons = new ReadOnlyDictionary<string, TrainObservingBeaconBase>(trainObservingBeacons);
            PreTrainObservingBeacons = new ReadOnlyDictionary<string, BeaconBase>(preTrainObservingBeacons);
        }

        public static ExtendedBeaconSet Load(BveHacker bveHacker, IDictionary<string, MapObjectList> repeatedStructures, IDictionary<string, Model> structureModels, IDictionary<string, Train> trains)
        {
            SortedList<string, BeaconBase> beacons = new SortedList<string, BeaconBase>();
            SortedList<string, TrainObservingBeaconBase> trainObservingBeacons = new SortedList<string, TrainObservingBeaconBase>();
            SortedList<string, BeaconBase> preTrainObservingBeacons = new SortedList<string, BeaconBase>();

            IEnumerable<KeyValuePair<string, RepeatedStructure>> flattenRepeatedStructures = repeatedStructures.
                Select(item => item.Value.Select(obj => new KeyValuePair<string, RepeatedStructure>(item.Key, obj as RepeatedStructure))).
                SelectMany(item => item);

            foreach (KeyValuePair<string, RepeatedStructure> repeater in flattenRepeatedStructures)
            {
                string repeaterKey = repeater.Key;
                RepeatedStructure repeatedStructure = repeater.Value;

                if (repeatedStructure.Models is null) continue;
                if (!repeatedStructure.Models.Any()) continue;

                {
                    string definerText = structureModels.TryGetKey(repeatedStructure.Models[0]);
                    if (definerText != Identifiers.Definer) continue;
                }

                if (repeatedStructure.Models.Count <= 4) throw new BveFileLoadException(Resources.GetString("ArgumentMissing").Value, Resources.GetString("ItemName").Value);

                string name = structureModels.TryGetKey(repeatedStructure.Models[1]);
                if (name == "atsex.null") name = Guid.NewGuid().ToString();

                string scriptLanguageText = structureModels.TryGetKey(repeatedStructure.Models[2]);
                if (!ScriptIdentifiers.ScriptLanguages.TryGetKey(scriptLanguageText, out ScriptLanguage scriptLanguage))
                {
                    throw new BveFileLoadException(ScriptIdentifiers.ErrorTexts.InvalidScriptLanguage(scriptLanguageText), Resources.GetString("ItemName").Value);
                }

                string trackObservingTypeText = structureModels.TryGetKey(repeatedStructure.Models[3]);
                if (!Identifiers.ObservingTargetTracks.TryGetKey(trackObservingTypeText, out ObservingTargetTrack observingTargetTrack))
                {
                    throw new BveFileLoadException(Identifiers.ErrorTexts.InvalidObservingTargetTrack(trackObservingTypeText), Resources.GetString("ItemName").Value);
                }

                for (int i = 4; i < repeatedStructure.Models.Count; i++)
                {
                    string observeTargetText = structureModels.TryGetKey(repeatedStructure.Models[i]);
                    if (!Identifiers.ObservingTargetTrains.TryGetKey(observeTargetText, out ObservingTargetTrain observingTargetTrain))
                    {
                        throw new BveFileLoadException(Identifiers.ErrorTexts.InvalidObservingTargetTrain(observeTargetText), Resources.GetString("ItemName").Value);
                    }

                    string code = Regex.Unescape(repeaterKey).Replace('`', '\"');

                    switch (observingTargetTrain)
                    {
                        case ObservingTargetTrain.Myself:
                        {
                            IPluginScript<ExtendedBeaconGlobalsBase<PassedEventArgs>> script = CreateScript<PassedEventArgs>(code, scriptLanguage);
                            Beacon beacon = new Beacon(bveHacker, name, repeatedStructure, observingTargetTrack, script);

                            beacons[name] = beacon;
                            break;
                        }

                        case ObservingTargetTrain.Trains:
                        {
                            IPluginScript<ExtendedBeaconGlobalsBase<TrainPassedEventArgs>> script = CreateScript<TrainPassedEventArgs>(code, scriptLanguage);
                            TrainObservingBeacon beacon = new TrainObservingBeacon(bveHacker, name, repeatedStructure, observingTargetTrack, trains, script);

                            trainObservingBeacons[name] = beacon;
                            break;
                        }

                        case ObservingTargetTrain.PreTrain:
                        {
                            IPluginScript<ExtendedBeaconGlobalsBase<PassedEventArgs>> script = CreateScript<PassedEventArgs>(code, scriptLanguage);
                            Beacon beacon = new Beacon(bveHacker, name, repeatedStructure, observingTargetTrack, script);

                            preTrainObservingBeacons[name] = beacon;
                            break;
                        }
                    }
                }
            }

            return new ExtendedBeaconSet(beacons, trainObservingBeacons, preTrainObservingBeacons);


            IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> CreateScript<TPassedEventArgs>(string code, ScriptLanguage language) where TPassedEventArgs : PassedEventArgs
            {
                string mainMapDirectory = Path.GetDirectoryName(bveHacker.ScenarioInfo.RouteFiles.SelectedFile.Path);

                IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> script;
                switch (language)
                {
                    case ScriptLanguage.CSharpScript:
                        script = new Plugins.Scripting.CSharp.PluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>>(code, mainMapDirectory, Resources.GetString("ItemName").Value);
                        break;

                    case ScriptLanguage.IronPython2:
                        ScriptEngine scriptEngine = ScriptEngineProvider.CreateEngine(mainMapDirectory);
                        ScriptScope scriptScope = ScriptEngineProvider.CreateScope(scriptEngine);

                        script = new Plugins.Scripting.IronPython2.PluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>>(code, scriptEngine, scriptScope, Resources.GetString("ItemName").Value);
                        break;

                    default:
                        throw new DevelopException(string.Format(Resources.GetString("ScriptLanguageNotRecognized").Value, language));
                }

                return script.GetWithCheckErrors();
            }
        }

        public void Tick(double location, double preTrainLocation)
        {
            foreach (Beacon beacon in Beacons.Values) beacon.Tick(location);
            foreach (TrainObservingBeacon beacon in TrainObservingBeacons.Values) beacon.Tick();
            foreach (Beacon beacon in PreTrainObservingBeacons.Values) beacon.Tick(preTrainLocation);
        }
    }
}
