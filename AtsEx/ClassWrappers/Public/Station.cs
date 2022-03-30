using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal sealed class Station : ClassWrapper, IStation
    {
        static Station()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IStation>();

            NameGetMethod = members.GetSourcePropertyGetterOf(nameof(Name));
            NameSetMethod = members.GetSourcePropertySetterOf(nameof(Name));

            ArrivalTimeGetMethod = members.GetSourcePropertyGetterOf(nameof(ArrivalTime));
            ArrivalTimeSetMethod = members.GetSourcePropertySetterOf(nameof(ArrivalTime));

            DepertureTimeGetMethod = members.GetSourcePropertyGetterOf(nameof(DepertureTime));
            DepertureTimeSetMethod = members.GetSourcePropertySetterOf(nameof(DepertureTime));

            DoorCloseTimeGetMethod = members.GetSourcePropertyGetterOf(nameof(DoorCloseTime));
            DoorCloseTimeSetMethod = members.GetSourcePropertySetterOf(nameof(DoorCloseTime));

            DefaultTimeGetMethod = members.GetSourcePropertyGetterOf(nameof(DefaultTime));
            DefaultTimeSetMethod = members.GetSourcePropertySetterOf(nameof(DefaultTime));

            PassGetMethod = members.GetSourcePropertyGetterOf(nameof(Pass));
            PassSetMethod = members.GetSourcePropertySetterOf(nameof(Pass));

            IsTerminalGetMethod = members.GetSourcePropertyGetterOf(nameof(IsTerminal));
            IsTerminalSetMethod = members.GetSourcePropertySetterOf(nameof(IsTerminal));

            StoppageTimeGetMethod = members.GetSourcePropertyGetterOf(nameof(StoppageTime));
            StoppageTimeSetMethod = members.GetSourcePropertySetterOf(nameof(StoppageTime));

            DoorSideGetMethod = members.GetSourcePropertyGetterOf(nameof(DoorSide));
            DoorSideSetMethod = members.GetSourcePropertySetterOf(nameof(DoorSide));

            DepertureSoundGetMethod = members.GetSourcePropertyGetterOf(nameof(DepertureSound));
            DepertureSoundSetMethod = members.GetSourcePropertySetterOf(nameof(DepertureSound));

            ArrivalSoundGetMethod = members.GetSourcePropertyGetterOf(nameof(ArrivalSound));
            ArrivalSoundSetMethod = members.GetSourcePropertySetterOf(nameof(ArrivalSound));

            SignalFlagGetMethod = members.GetSourcePropertyGetterOf(nameof(SignalFlag));
            SignalFlagSetMethod = members.GetSourcePropertySetterOf(nameof(SignalFlag));

            MarginMaxGetMethod = members.GetSourcePropertyGetterOf(nameof(MarginMax));
            MarginMaxSetMethod = members.GetSourcePropertySetterOf(nameof(MarginMax));

            MarginMinGetMethod = members.GetSourcePropertyGetterOf(nameof(MarginMin));
            MarginMinSetMethod = members.GetSourcePropertySetterOf(nameof(MarginMin));

            MinStopPositionGetMethod = members.GetSourcePropertyGetterOf(nameof(MinStopPosition));

            MaxStopPositionGetMethod = members.GetSourcePropertyGetterOf(nameof(MaxStopPosition));

            AlightingTimeGetMethod = members.GetSourcePropertyGetterOf(nameof(AlightingTime));
            AlightingTimeSetMethod = members.GetSourcePropertySetterOf(nameof(AlightingTime));

            TargetLoadFactorGetMethod = members.GetSourcePropertyGetterOf(nameof(TargetLoadFactor));
            TargetLoadFactorSetMethod = members.GetSourcePropertySetterOf(nameof(TargetLoadFactor));

            CurrentLoadFactorGetMethod = members.GetSourcePropertyGetterOf(nameof(CurrentLoadFactor));
            CurrentLoadFactorSetMethod = members.GetSourcePropertySetterOf(nameof(CurrentLoadFactor));

            DoorReopenGetMethod = members.GetSourcePropertyGetterOf(nameof(DoorReopen));
            DoorReopenSetMethod = members.GetSourcePropertySetterOf(nameof(DoorReopen));

            StuckInDoorGetMethod = members.GetSourcePropertyGetterOf(nameof(StuckInDoor));
            StuckInDoorSetMethod = members.GetSourcePropertySetterOf(nameof(StuckInDoor));
        }

        public Station(object src) : base(src)
        {
        }

        private static MethodInfo NameGetMethod;
        private static MethodInfo NameSetMethod;
        public string Name
        {
            get => NameGetMethod.Invoke(Src, null);
            set => NameSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo ArrivalTimeGetMethod;
        private static MethodInfo ArrivalTimeSetMethod;
        public int ArrivalTime
        {
            get => ArrivalTimeGetMethod.Invoke(Src, null);
            set => ArrivalTimeSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo DepertureTimeGetMethod;
        private static MethodInfo DepertureTimeSetMethod;
        public int DepertureTime
        {
            get => DepertureTimeGetMethod.Invoke(Src, null);
            set => DepertureTimeSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo DoorCloseTimeGetMethod;
        private static MethodInfo DoorCloseTimeSetMethod;
        public int DoorCloseTime
        {
            get => DoorCloseTimeGetMethod.Invoke(Src, null);
            set => DoorCloseTimeSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo DefaultTimeGetMethod;
        private static MethodInfo DefaultTimeSetMethod;
        public int DefaultTime
        {
            get => DefaultTimeGetMethod.Invoke(Src, null);
            set => DefaultTimeSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo PassGetMethod;
        private static MethodInfo PassSetMethod;
        public bool Pass
        {
            get => PassGetMethod.Invoke(Src, null);
            set => PassSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo IsTerminalGetMethod;
        private static MethodInfo IsTerminalSetMethod;
        public bool IsTerminal
        {
            get => IsTerminalGetMethod.Invoke(Src, null);
            set => IsTerminalSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo StoppageTimeGetMethod;
        private static MethodInfo StoppageTimeSetMethod;
        public int StoppageTime
        {
            get => StoppageTimeGetMethod.Invoke(Src, null);
            set => StoppageTimeSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo DoorSideGetMethod;
        private static MethodInfo DoorSideSetMethod;
        public int DoorSide
        {
            get => DoorSideGetMethod.Invoke(Src, null);
            set => DoorSideSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo DepertureSoundGetMethod;
        private static MethodInfo DepertureSoundSetMethod;
        public ISound DepertureSound
        {
            get => new Sound(DepertureSoundGetMethod.Invoke(Src, null));
            set => DepertureSoundSetMethod.Invoke(Src, new object[] { value.Src });
        }

        private static MethodInfo ArrivalSoundGetMethod;
        private static MethodInfo ArrivalSoundSetMethod;
        public ISound ArrivalSound
        {
            get => new Sound(ArrivalSoundGetMethod.Invoke(Src, null));
            set => ArrivalSoundSetMethod.Invoke(Src, new object[] { value.Src });
        }

        private static MethodInfo SignalFlagGetMethod;
        private static MethodInfo SignalFlagSetMethod;
        public bool SignalFlag
        {
            get => SignalFlagGetMethod.Invoke(Src, null);
            set => SignalFlagSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo MarginMaxGetMethod;
        private static MethodInfo MarginMaxSetMethod;
        public double MarginMax
        {
            get => MarginMaxGetMethod.Invoke(Src, null);
            set => MarginMaxSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo MarginMinGetMethod;
        private static MethodInfo MarginMinSetMethod;
        public double MarginMin
        {
            get => MarginMinGetMethod.Invoke(Src, null);
            set => MarginMinSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo MinStopPositionGetMethod;
        public double MinStopPosition
        {
            get => MinStopPositionGetMethod.Invoke(Src, null);
        }

        private static MethodInfo MaxStopPositionGetMethod;
        public double MaxStopPosition
        {
            get => MaxStopPositionGetMethod.Invoke(Src, null);
        }

        private static MethodInfo AlightingTimeGetMethod;
        private static MethodInfo AlightingTimeSetMethod;

        public int AlightingTime
        {
            get => AlightingTimeGetMethod.Invoke(Src, null);
            set => AlightingTimeSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo TargetLoadFactorGetMethod;
        private static MethodInfo TargetLoadFactorSetMethod;
        public double TargetLoadFactor
        {
            get => TargetLoadFactorGetMethod.Invoke(Src, null);
            set => TargetLoadFactorSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo CurrentLoadFactorGetMethod;
        private static MethodInfo CurrentLoadFactorSetMethod;
        public double CurrentLoadFactor
        {
            get => CurrentLoadFactorGetMethod.Invoke(Src, null);
            set => CurrentLoadFactorSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo DoorReopenGetMethod;
        private static MethodInfo DoorReopenSetMethod;
        public double DoorReopen
        {
            get => DoorReopenGetMethod.Invoke(Src, null);
            set => DoorReopenSetMethod.Invoke(Src, new object[] { value });
        }

        private static MethodInfo StuckInDoorGetMethod;
        private static MethodInfo StuckInDoorSetMethod;
        public double StuckInDoor
        {
            get => StuckInDoorGetMethod.Invoke(Src, null);
            set => StuckInDoorSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
