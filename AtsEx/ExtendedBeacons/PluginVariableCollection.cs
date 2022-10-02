using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using Automatic9045.AtsEx.PluginHost.Plugins;

namespace Automatic9045.AtsEx.ExtendedBeacons
{
    public sealed class PluginVariableCollection
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<PluginVariableCollection>(@"Core\ExtendedBeacons");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> PluginIdentifierNotFound { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly ResourceSet Resources = new ResourceSet();

        private readonly IEnumerable<string> PluginIdentifiers;
        private readonly PluginType PluginType;

        private readonly SortedList<string, SortedList<string, dynamic>> Variables = new SortedList<string, SortedList<string, dynamic>>();

        public PluginVariableCollection(IEnumerable<string> pluginIdentifiers, PluginType pluginType)
        {
            PluginIdentifiers = pluginIdentifiers;
            PluginType = pluginType;
        }

        public T GetPluginVariable<T>(string pluginIdentifier, string name) => (T)Variables[pluginIdentifier][name];

        public void SetPluginVariable<T>(string pluginIdentifier, string name, T value)
        {
            if (!Variables.ContainsKey(pluginIdentifier))
            {
                if (!PluginIdentifiers.Contains(pluginIdentifier))
                {
                    throw new KeyNotFoundException(string.Format(Resources.PluginIdentifierNotFound.Value, PluginType.GetTypeString(), pluginIdentifier));
                }

                Variables[pluginIdentifier] = new SortedList<string, dynamic>();
            }

            Variables[pluginIdentifier][name] = value;
        }
    }
}
