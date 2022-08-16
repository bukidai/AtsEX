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
using Automatic9045.AtsEx.PluginHost.Plugins;
using Automatic9045.AtsEx.PluginHost.Resources;

using Identifiers = Automatic9045.AtsEx.ExtendedBeacons.MapStatementIdentifiers;
using ScriptIdentifiers = Automatic9045.AtsEx.Plugins.Scripting.MapStatementIdentifiers;

using BeaconBase = Automatic9045.AtsEx.PluginHost.ExtendedBeacons.ExtendedBeaconBase<Automatic9045.AtsEx.PluginHost.ExtendedBeacons.PassedEventArgs>;
using TrainObservingBeaconBase = Automatic9045.AtsEx.PluginHost.ExtendedBeacons.ExtendedBeaconBase<Automatic9045.AtsEx.PluginHost.ExtendedBeacons.TrainPassedEventArgs>;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal partial class ExtendedBeaconSet : PluginHost.ExtendedBeacons.ExtendedBeaconSet
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<ExtendedBeaconSet>(@"Core\ExtendedBeacons");

        public override ReadOnlyDictionary<string, BeaconBase> Beacons { get; }
        public override ReadOnlyDictionary<string, TrainObservingBeaconBase> TrainObservingBeacons { get; }
        public override ReadOnlyDictionary<string, BeaconBase> PreTrainObservingBeacons { get; }

        protected SortedList<PluginType, PluginVariableCollection> PluginVariables = new SortedList<PluginType, PluginVariableCollection>();

        protected ExtendedBeaconSet(SortedList<PluginType, PluginVariableCollection> pluginVariables,
            IDictionary<string, BeaconBase> beacons, IDictionary<string, TrainObservingBeaconBase> trainObservingBeacons, IDictionary<string, BeaconBase> preTrainObservingBeacons)
        {
            PluginVariables = pluginVariables;

            Beacons = new ReadOnlyDictionary<string, BeaconBase>(beacons);
            TrainObservingBeacons = new ReadOnlyDictionary<string, TrainObservingBeaconBase>(trainObservingBeacons);
            PreTrainObservingBeacons = new ReadOnlyDictionary<string, BeaconBase>(preTrainObservingBeacons);
        }

        public static ExtendedBeaconSet Load(BveHacker bveHacker, IDictionary<string, MapObjectList> repeatedStructures, IDictionary<string, Model> structureModels, IDictionary<string, Train> trains)
        {
            SortedList<PluginType, PluginVariableCollection> pluginVariables = new SortedList<PluginType, PluginVariableCollection>();
            foreach (PluginType pluginType in Enum.GetValues(typeof(PluginType)))
            {
                pluginVariables[pluginType] = new PluginVariableCollection(App.Instance.Plugins[pluginType].Keys, pluginType);
            }

            Dictionary<string, BeaconBase> beacons = new Dictionary<string, BeaconBase>();
            Dictionary<string, TrainObservingBeaconBase> trainObservingBeacons = new Dictionary<string, TrainObservingBeaconBase>();
            Dictionary<string, BeaconBase> preTrainObservingBeacons = new Dictionary<string, BeaconBase>();

            List<ICompilationErrorCheckable> errorCheckList = new List<ICompilationErrorCheckable>();

            Dictionary<Type, SortedList<ScriptLanguage, SortedList<string, dynamic>>> cachedScripts = new Dictionary<Type, SortedList<ScriptLanguage, SortedList<string, dynamic>>>();

            foreach (KeyValuePair<string, MapObjectList> sameKeyRepeaters in repeatedStructures)
            {
                string repeaterKey = sameKeyRepeaters.Key;

                foreach (RepeatedStructure repeatedStructure in sameKeyRepeaters.Value)
                {
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
                                Beacon beacon = repeatedStructure.Location < 0
                                    ? new InitializerBeacon(bveHacker, pluginVariables, name, repeatedStructure, observingTargetTrack, observingTargetTrain, script)
                                    : new Beacon(bveHacker, pluginVariables, name, repeatedStructure, observingTargetTrack, observingTargetTrain, script);

                                beacons[name] = beacon;
                                errorCheckList.Add(beacon);
                                break;
                            }

                            case ObservingTargetTrain.Trains:
                            {
                                IPluginScript<ExtendedBeaconGlobalsBase<TrainPassedEventArgs>> script = CreateScript<TrainPassedEventArgs>(code, scriptLanguage);
                                TrainObservingBeacon beacon = new TrainObservingBeacon(bveHacker, pluginVariables, name, repeatedStructure, observingTargetTrack, trains, script);

                                trainObservingBeacons[name] = beacon;
                                errorCheckList.Add(beacon);
                                break;
                            }

                            case ObservingTargetTrain.PreTrain:
                            {
                                IPluginScript<ExtendedBeaconGlobalsBase<PassedEventArgs>> script = CreateScript<PassedEventArgs>(code, scriptLanguage);
                                Beacon beacon = new Beacon(bveHacker, pluginVariables, name, repeatedStructure, observingTargetTrack, observingTargetTrain, script);

                                preTrainObservingBeacons[name] = beacon;
                                errorCheckList.Add(beacon);
                                break;
                            }
                        }
                    }
                }
            }

            errorCheckList.ForEach(beacon => beacon.CheckCompilationErrors());

            return new ExtendedBeaconSet(pluginVariables, OrderBeacons(beacons), OrderBeacons(trainObservingBeacons), OrderBeacons(preTrainObservingBeacons));


            IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> CreateScript<TPassedEventArgs>(string code, ScriptLanguage language) where TPassedEventArgs : PassedEventArgs
            {
                string mainMapDirectory = Path.GetDirectoryName(bveHacker.ScenarioInfo.RouteFiles.SelectedFile.Path);

                if (!cachedScripts.ContainsKey(typeof(TPassedEventArgs))) cachedScripts[typeof(TPassedEventArgs)] = new SortedList<ScriptLanguage, SortedList<string, dynamic>>();
                if (!cachedScripts[typeof(TPassedEventArgs)].ContainsKey(language)) cachedScripts[typeof(TPassedEventArgs)][language] = new SortedList<string, dynamic>();

                if (cachedScripts[typeof(TPassedEventArgs)][language].ContainsKey(code))
                {
                    IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>> cachedScript = cachedScripts[typeof(TPassedEventArgs)][language][code] as IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>>;
                    return cachedScript.Clone() as IPluginScript<ExtendedBeaconGlobalsBase<TPassedEventArgs>>;
                }

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

                cachedScripts[typeof(TPassedEventArgs)][language][code] = script;

                return script;
            }

            Dictionary<string, TBeacon> OrderBeacons<TBeacon>(Dictionary<string, TBeacon> source) where TBeacon : IExtendedBeacon
                => source.OrderBy(item => item.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        protected override T GetPluginVariable<T>(PluginBase target, string name) => PluginVariables[target.PluginType].GetPluginVariable<T>(target.Identifier, name);

        public void Tick(double location, double preTrainLocation)
        {
            foreach (Beacon beacon in Beacons.Values) beacon.Tick(location);
            foreach (TrainObservingBeacon beacon in TrainObservingBeacons.Values) beacon.Tick();
            foreach (Beacon beacon in PreTrainObservingBeacons.Values) beacon.Tick(preTrainLocation);
        }
    }
}
