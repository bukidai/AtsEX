using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
	public interface IRoute : IClassWrapper
	{
		double DrawLimitLocation { get; set; }
		IStructureSet Structures { get; }
		IWrappedSortedList<string, ISound> Sounds { get; }
		IWrappedSortedList<string, ISound[]> Sounds3D { get; }
		IWrappedSortedList<string, IModel> StructureModels { get; }
	}
}
