using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zbx1425.DXDynamicTexture;

namespace Automatic9045.AtsEx.PluginHost
{
    internal class DXDynamicTextureHost : IDisposable
    {
        public void Initiaize()
        {
            TextureManager.Initialize();
        }

        public void Dispose()
        {
            TextureManager.Dispose();
        }
    }
}
