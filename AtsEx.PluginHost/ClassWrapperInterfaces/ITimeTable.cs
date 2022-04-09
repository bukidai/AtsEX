using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
	public interface ITimeTable : IAssistantText
	{
		IModel Model { get; set; }
		string[] NameTexts { get; set; }
		string[] ArrivalTimeTexts { get; set; }
		string[] DepertureTimeTexts { get; set; }
		int[] NameTextWidths { get; set; }
		int[] ArrivalTimeTextWidths { get; set; }
		int[] DepertureTimeTextWidths { get; set; }
		void Update();
	}
}
