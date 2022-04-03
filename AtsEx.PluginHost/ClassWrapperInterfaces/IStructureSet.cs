using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
	public interface IStructureSet : IClassWrapper
	{
		double DrawLimitLocation { get; set; }
		IWrappedSortedList<string, IMapObjectList> Repeated { get; }
		ISingleStructureList Put { get; }
		ISingleStructureList PutBetween { get; }
		ISingleStructureList Signals { get; }
	}
}
