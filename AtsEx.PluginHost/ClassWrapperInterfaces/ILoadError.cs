using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
	public interface ILoadError : IClassWrapper
	{
		string Text { get; }
		string SenderFileName { get; }
		int LineIndex { get; }
		int CharIndex { get; }
	}
}
