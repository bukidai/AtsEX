using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Input;

namespace Automatic9045.AtsEx.Input
{
    internal sealed class NativeAtsKey : KeyBase
    {
        public NativeAtsKey() : base()
        {
        }

        internal new void Press() => base.Press();
        internal new void Release() => base.Release();
    }
}
