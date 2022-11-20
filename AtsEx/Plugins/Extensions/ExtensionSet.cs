using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

namespace AtsEx.Plugins.Extensions
{
    internal class ExtensionSet : IExtensionSet
    {
        private readonly Dictionary<Type, PluginBase> Extensions;

        public ExtensionSet(IEnumerable<PluginBase> extensions)
        {
            Extensions = extensions.ToDictionary(x => x.GetType(), x => x);
        }

        public TExtension GetExtension<TExtension>() where TExtension : PluginBase => (TExtension)Extensions[typeof(TExtension)];

        public IEnumerator<PluginBase> GetEnumerator() => Extensions.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
