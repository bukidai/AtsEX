using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX.Direct3D9;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
	public interface IMaterialInfo : IClassWrapper
	{
		Material Material { get; set; }
		Texture Texture { get; set; }
		bool Is2D { get; set; }
	}
}
