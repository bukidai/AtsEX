using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.Ats;
using Automatic9045.AtsEx.BveHackServices;
using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx
{
    internal class Vehicle : BveHackService, IVehicle
    {
        public Vehicle(BveHacker bveHacker, ServiceCollection services) : base(bveHacker, services)
        {
        }

        public double LocationD => AtsMain.VehicleState.Location;
        public int Location
        {
            get => (int)LocationD;
            set => Services.GetService<ILocationHacker>().Location = value;
        }

        public int Speed
        {
            get => Services.GetService<ISpeedHacker>().Speed;
            set => Services.GetService<ISpeedHacker>().Speed = value;
        }

        public float DisplaySpeed => AtsMain.VehicleState.Speed;

        public int BrakeNotches => AtsMain.VehicleSpec.BrakeNotches;
        public int PowerNotches => AtsMain.VehicleSpec.PowerNotches;
        public int AtsNotch => AtsMain.VehicleSpec.AtsNotch;
        public int B67Notch => AtsMain.VehicleSpec.B67Notch;
        public int Cars => AtsMain.VehicleSpec.Cars;

        public float BcPressure => AtsMain.VehicleState.BcPressure;
        public float MrPressure => AtsMain.VehicleState.MrPressure;
        public float ErPressure => AtsMain.VehicleState.ErPressure;
        public float BpPressure => AtsMain.VehicleState.BpPressure;
        public float SapPressure => AtsMain.VehicleState.SapPressure;
        public float Current => AtsMain.VehicleState.Current;

        public int BrakeNotch
        {
            get => AtsMain.Handle.Brake;
            set => AtsMain.Handle.Brake = value;
        }
        public int PowerNotch
        {
            get => AtsMain.Handle.Power;
            set => AtsMain.Handle.Power = value;
        }
        public PluginHost.ReverserPosition ReverserPosition
        {
            get => (PluginHost.ReverserPosition)AtsMain.Handle.Reverser;
            set => AtsMain.Handle.Reverser = (int)value;
        }
        public ConstantSpeedState ConstantSpeedState
        {
            get => (ConstantSpeedState)AtsMain.Handle.ConstantSpeed;
            set => AtsMain.Handle.ConstantSpeed = (int)value;
        }
    }
}
