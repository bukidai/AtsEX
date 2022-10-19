using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

namespace AtsEx.PluginHost
{
    public class PropertyNotInitializedException : Exception
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<PropertyNotInitializedException>("PluginHost");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> Message { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static PropertyNotInitializedException()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        public PropertyNotInitializedException(string propertyName) : base(string.Format(Resources.Value.Message.Value, propertyName))
        {
        }
    }
}
