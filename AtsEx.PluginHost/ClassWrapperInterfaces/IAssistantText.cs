using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
	public interface IAssistantText : IAssistantTextBase
	{
		Color Color { get; set; }
		string Text { get; set; }
	}
}
