using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.PluginHost.MapStatements;

namespace AtsEx.Extensions.ExtendedBeacons
{
    internal static partial class Identifiers
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType(typeof(Identifiers), @"ExtendedBeacons");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> InvalidObservingTargetTrack { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> InvalidObservingTargetTrain { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static Identifiers()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private static readonly Identifier Root = new Identifier(Namespace.Root, "extendedbeacon");

        public static readonly Identifier Put = new Identifier(Root, "put");

        private static readonly Identifier ObservingTargetTrackRoot = new Identifier(Root, "observingtargettrack");
        private static readonly IdentifierToEnumConverter<ObservingTargetTrack> ObservingTargetTrackConverter = new IdentifierToEnumConverter<ObservingTargetTrack>(ObservingTargetTrackRoot);

        private static readonly Identifier ObservingTargetTrainRoot = new Identifier(Root, "observingtargettrain");
        private static readonly IdentifierToEnumConverter<ObservingTargetTrain> ObservingTargetTrainConverter = new IdentifierToEnumConverter<ObservingTargetTrain>(ObservingTargetTrainRoot);

        public static bool TryConvertObservingTargetTrack(Identifier value, out ObservingTargetTrack result) => ObservingTargetTrackConverter.TryConvert(value, out result);

        public static ObservingTargetTrack ConvertObservingTargetTrack(Identifier value)
        {
            return TryConvertObservingTargetTrack(value, out ObservingTargetTrack result)
                ? result : throw new ArgumentException(string.Format(Resources.Value.InvalidObservingTargetTrack.Value, value.FullName));
        }

        public static bool TryConvertObservingTargetTrain(Identifier value, out ObservingTargetTrain result) => ObservingTargetTrainConverter.TryConvert(value, out result);

        public static ObservingTargetTrain ConvertObservingTargetTrain(Identifier value)
        {
            return TryConvertObservingTargetTrain(value, out ObservingTargetTrain result)
                ? result : throw new ArgumentException(string.Format(Resources.Value.InvalidObservingTargetTrain.Value, value.FullName));
        }
    }
}
