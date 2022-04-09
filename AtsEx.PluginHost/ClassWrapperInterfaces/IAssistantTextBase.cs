using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mackoy.Bvets;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
	public interface IAssistantTextBase : IClassWrapper
	{
		AssistantSettings AssistantSettings { get; }
		Color BackgroundColor { get; set; }
		Rectangle TextArea { get; }
		void Draw();
	}
}
