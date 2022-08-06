using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    internal struct Profile : IDisposable
    {
        public Stream Stream { get; }
        public Version Version { get; }

        public Profile(Stream stream, Version version)
        {
            Stream = stream;
            Version = version;
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
