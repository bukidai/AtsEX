using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.Hosting
{
    internal class ReleaseInfo
    {
        public Version Version { get; }

        private readonly Func<string> UpdateInfoMessageGetter;

        public ReleaseInfo(Version version, Func<string> updateInfoMessageGetter)
        {
            Version = version;
            UpdateInfoMessageGetter = updateInfoMessageGetter;
        }

        public string GetUpdateInfoMessage() => UpdateInfoMessageGetter();
    }
}
