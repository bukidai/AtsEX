using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zbx1425.DXDynamicTexture;

using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

namespace AtsEx.Extensions.DXDynamicTexture
{
    [HideExtensionMain]
    public class ExtensionMain : AssemblyPluginBase
    {
        public ExtensionMain(PluginBuilder builder) : base(builder, PluginType.Extension, false)
        {
            TextureManager.Initialize();
        }

        public override void Dispose()
        {
            TextureManager.Clear();
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            return new ExtensionTickResult();
        }
    }
}
