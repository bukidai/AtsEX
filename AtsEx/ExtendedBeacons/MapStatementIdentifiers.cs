using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.PluginHost.ExtendedBeacons;

namespace AtsEx.ExtendedBeacons
{
    internal static partial class MapStatementIdentifiers
    {
        public static readonly string Definer = "AtsEx::ExtendedBeacon".ToLower();

        private const string ObservingTargetTrackBase = "AtsEx::ExtendedBeacon.ObservingTargetTrack.";
        public static readonly ReadOnlyDictionary<ObservingTargetTrack, string> ObservingTargetTracks = new ReadOnlyDictionary<ObservingTargetTrack, string>(new Dictionary<ObservingTargetTrack, string>()
        {
            { ObservingTargetTrack.SpecifiedTrackOnly, (ObservingTargetTrackBase + "SpecifiedTrackOnly").ToLower() },
            { ObservingTargetTrack.AllTracks, (ObservingTargetTrackBase + "AllTracks").ToLower() },
        });

        private const string ObservingTargetTrainBase = "AtsEx::ExtendedBeacon.ObservingTargetTrain.";
        public static readonly ReadOnlyDictionary<ObservingTargetTrain, string> ObservingTargetTrains = new ReadOnlyDictionary<ObservingTargetTrain, string>(new Dictionary<ObservingTargetTrain, string>()
        {
            { PluginHost.ExtendedBeacons.ObservingTargetTrain.Myself, (ObservingTargetTrainBase + "Myself").ToLower() },
            { PluginHost.ExtendedBeacons.ObservingTargetTrain.Trains, (ObservingTargetTrainBase + "Trains").ToLower() },
            { PluginHost.ExtendedBeacons.ObservingTargetTrain.PreTrain, (ObservingTargetTrainBase + "PreTrain").ToLower() },
        });

        internal static class ErrorTexts
        {
            private class ResourceSet
            {
                private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType(typeof(MapStatementIdentifiers), @"Core\ExtendedBeacons");

                [ResourceStringHolder(nameof(Localizer))] public Resource<string> InvalidObservingTargetTrack { get; private set; }
                [ResourceStringHolder(nameof(Localizer))] public Resource<string> InvalidObservingTargetTrain { get; private set; }

                public ResourceSet()
                {
                    ResourceLoader.LoadAndSetAll(this);
                }
            }

            private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

            static ErrorTexts()
            {
#if DEBUG
                _ = Resources.Value;
#endif
            }

            public static string InvalidObservingTargetTrack(string invalidText) => string.Format(Resources.Value.InvalidObservingTargetTrack.Value, invalidText);
            public static string InvalidObservingTargetTrain(string invalidText) => string.Format(Resources.Value.InvalidObservingTargetTrain.Value, invalidText);
        }
    }
}
