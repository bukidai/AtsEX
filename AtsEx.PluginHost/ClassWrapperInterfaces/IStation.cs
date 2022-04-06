using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
	public interface IStation : IMapObjectBase
	{
		string Name { get; set; }
		int ArrivalTime { get; set; }
		int DepertureTime { get; set; }
		int DoorCloseTime { get; set; }
		int DefaultTime { get; set; }
		bool Pass { get; set; }
		bool IsTerminal { get; set; }
		int StoppageTime { get; set; }
		int DoorSide { get; set; }
		ISound DepertureSound { get; set; }
		ISound ArrivalSound { get; set; }
		bool SignalFlag { get; set; }
		double MarginMax { get; set; }
		double MarginMin { get; set; }
		double MinStopPosition { get; }
		double MaxStopPosition { get; }
		int AlightingTime { get; set; }
		double TargetLoadFactor { get; set; }
		double CurrentLoadFactor { get; set; }
		double DoorReopen { get; set; }
		double StuckInDoor { get; set; }
	}

	public enum DoorSide
    {
		Left = -1,
		DoNotOpen = 0,
		Right = 1,
    }
}
