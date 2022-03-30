using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX.Direct3D9;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
	public interface IModel : IClassWrapper
	{
		Mesh Mesh { get; set; }
		IMaterialInfo[]	Materials { get; set; }
	}
}
