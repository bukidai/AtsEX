using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using Automatic9045.AtsEx.PluginHost.ExtendedBeacons;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    internal static partial class MapStatementIdentifiers
    {
        public static readonly string Definer = "AtsEx::ExtendedBeacon".ToLower();

        private const string ObservingTargetTrackBase = "AtsEx::ExtendedBeacon.ObservingTargetTrack.";
        public static readonly ReadOnlyDictionary<ObservingTargetTrack, string> ObservingTargetTracks = new ReadOnlyDictionary<ObservingTargetTrack, string>(new SortedList<ObservingTargetTrack, string>()
        {
            { ObservingTargetTrack.SpecifiedTrackOnly, (ObservingTargetTrackBase + "SpecifiedTrackOnly").ToLower() },
            { ObservingTargetTrack.AllTracks, (ObservingTargetTrackBase + "AllTracks").ToLower() },
        });

        private const string ObservingTargetTrainBase = "AtsEx::ExtendedBeacon.ObservingTargetTrain.";
        public static readonly ReadOnlyDictionary<ObservingTargetTrain, string> ObservingTargetTrains = new ReadOnlyDictionary<ObservingTargetTrain, string>(new SortedList<ObservingTargetTrain, string>()
        {
            { PluginHost.ExtendedBeacons.ObservingTargetTrain.Myself, (ObservingTargetTrainBase + "Myself").ToLower() },
            { PluginHost.ExtendedBeacons.ObservingTargetTrain.Trains, (ObservingTargetTrainBase + "Trains").ToLower() },
            { PluginHost.ExtendedBeacons.ObservingTargetTrain.PreTrain, (ObservingTargetTrainBase + "PreTrain").ToLower() },
        });

        internal static class ErrorTexts
        {
            private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(MapStatementIdentifiers), @"Core\ExtendedBeacons");

            public static string InvalidObservingTargetTrack(string invalidText) => string.Format(Resources.GetString("InvalidObservingTargetTrack").Value, invalidText);
            public static string InvalidObservingTargetTrain(string invalidText) => string.Format(Resources.GetString("InvalidObservingTargetTrain").Value, invalidText);
        }
    }
}
