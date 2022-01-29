using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    public class LoadError : ClassWrapper, ILoadError
    {
        public LoadError(object src) : base(src)
        {
        }

		public string Text
		{
			get => Src.b;
			internal set => Src.b(value);
		}

		public string SenderPath
		{
			get => Src.c;
			internal set => Src.a(value);
		}

		public int LineIndex
		{
			get => Src.d;
			internal set => Src.b(value);
		}

		public int CharIndex
		{
			get => Src.a;
			internal set => Src.a(value);
		}
	}
}
