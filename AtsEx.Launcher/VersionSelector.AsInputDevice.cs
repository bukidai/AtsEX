using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.Launcher
{
    public partial class VersionSelector
    {
        public class AsInputDevice : VersionSelector
        {
            public new CoreHostAsInputDevice CoreHost { get; }

            public AsInputDevice(Assembly callerAssembly) : base()
            {
                CoreHost = new CoreHostAsInputDevice(callerAssembly, BveFinder);
            }
        }
    }
}
