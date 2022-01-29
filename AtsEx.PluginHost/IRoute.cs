using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost
{
    public interface IRoute
    {
        int TimeMilliseconds { get; }
        DateTime Time { get; set; }
    }
}
