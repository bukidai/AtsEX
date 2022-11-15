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
    internal class ExtensionFactorySet : IExtensionFactorySet
    {
        private readonly Dictionary<Type, PluginBase> ExtensionFactories;

        public ExtensionFactorySet(IEnumerable<PluginBase> extensionFactories)
        {
            ExtensionFactories = extensionFactories.ToDictionary(x => x.GetType(), x => x);
        }

        public TExtensionFactory GetFactory<TExtensionFactory>() where TExtensionFactory : PluginBase => (TExtensionFactory)ExtensionFactories[typeof(TExtensionFactory)];

        public IEnumerator<PluginBase> GetEnumerator() => ExtensionFactories.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
