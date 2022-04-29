using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class ChartForm : ClassWrapper
    {
        private ChartForm(object src) : base(src)
        {
        }

        public static ChartForm FromSource(object src)
        {
            if (src is null) return null;
            return new ChartForm(src);
        }
    }
}
