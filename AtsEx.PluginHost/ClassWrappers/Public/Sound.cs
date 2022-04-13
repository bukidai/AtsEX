using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public sealed class Sound : ClassWrapper
    {
        static Sound()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<Sound>();
        }

        private Sound(object src) : base(src)
        {
        }

        public static Sound FromSource(object src) => new Sound(src);
    }
}
