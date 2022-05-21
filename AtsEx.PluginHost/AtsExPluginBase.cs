using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.PluginHost
{
    /// <summary>
    /// AtsEX プラグインを表します。
    /// </summary>
    public abstract class AtsExPluginBase
    {
        protected IApp App { get; }
        protected IBveHacker BveHacker { get; }

        public AtsExPluginBase(HostServiceCollection services)
        {
            App = services.App;
            BveHacker = services.BveHacker;
        }

        /// <summary>
        /// 毎フレーム呼び出されます。従来の ATS プラグインの Elapse(ATS_VEHICLESTATE vehicleState, int[] panel, int[] sound) に当たります。
        /// </summary>
        public abstract void Tick();
    }
}
