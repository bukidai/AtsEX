using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Resources
{
    /// <summary>
    /// <see cref="CultureInfo"/> を、そのカルチャの LCID によって比較します。
    /// </summary>
    public class CultureComparer : IComparer<CultureInfo>
    {
        /// <inheritdoc/>
        public int Compare(CultureInfo x, CultureInfo y) => x.LCID.CompareTo(y.LCID);
    }
}
