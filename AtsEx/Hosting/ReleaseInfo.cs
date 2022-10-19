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

        private readonly Lazy<string> _UpdateInfoMessage;
        public string UpdateInfoMessage => _UpdateInfoMessage.Value;

        public ReleaseInfo(Version version, Func<string> updateInfoMessageGetter)
        {
            Version = version;
            _UpdateInfoMessage = new Lazy<string>(updateInfoMessageGetter);
        }
    }
}
