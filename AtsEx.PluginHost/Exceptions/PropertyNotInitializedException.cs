using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

namespace Automatic9045.AtsEx.PluginHost
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

        private static readonly ResourceSet Resources = new ResourceSet();

        public PropertyNotInitializedException(string propertyName) : base(string.Format(Resources.Message.Value, propertyName))
        {
        }
    }
}
