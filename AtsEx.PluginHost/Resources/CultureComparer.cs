using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Resources
{
    public class CultureComparer : IComparer<CultureInfo>
    {
        public int Compare(CultureInfo x, CultureInfo y) => x.LCID.CompareTo(y.LCID);
    }
}
