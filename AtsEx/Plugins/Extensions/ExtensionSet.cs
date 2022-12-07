using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

namespace AtsEx.Plugins.Extensions
{
    internal class ExtensionSet : IExtensionSet
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType<ExtensionSet>(@"Core\Plugins\Extensions");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> DisplayTypeNotExtension { get; private set; }
            [ResourceStringHolder(nameof(Localizer))] public Resource<string> NotSubclassOfDisplayType { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static ExtensionSet()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private readonly Dictionary<Type, ExtensionDefinitionInfo> Extensions;

        public ExtensionSet(IEnumerable<PluginBase> extensions)
        {
            Extensions = extensions.Select(x =>
            {
                Type type = x.GetType();

                Type displayType = type;
                bool hide = false;
                foreach (Attribute attribute in type.GetCustomAttributes())
                {
                    switch (attribute)
                    {
                        case HideExtensionMainAttribute hideAttribute:
                            hide = true;
                            break;

                        case ExtensionMainDisplayTypeAttribute displayTypeAttribute:
                            displayType = displayTypeAttribute.DisplayType;
                            break;
                    }
                }

                return new ExtensionDefinitionInfo(x, hide, displayType);
            }).ToDictionary(x => x.DisplayType, x => x);
        }

        public TExtension GetExtension<TExtension>() where TExtension : IExtension
        {
            ExtensionDefinitionInfo result = Extensions[typeof(TExtension)];
            return !result.Hide && result.Body is TExtension extension ? extension : throw new KeyNotFoundException();
        }

        public IEnumerator<PluginBase> GetEnumerator() => Extensions.Values.Select(x => x.Body).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        private class ExtensionDefinitionInfo
        {
            public PluginBase Body { get; }
            public bool Hide { get; }
            public Type DisplayType { get; }

            public ExtensionDefinitionInfo(PluginBase body, bool hide, Type displayType)
            {
                if (!displayType.GetInterfaces().Contains(typeof(IExtension)))
                {
                    throw new InvalidCastException(string.Format(Resources.Value.DisplayTypeNotExtension.Value, nameof(displayType), displayType.FullName, typeof(IExtension).FullName));
                }

                Type bodyType = body.GetType();
                if (bodyType != displayType && !bodyType.GetInterfaces().Contains(displayType) && !bodyType.IsSubclassOf(displayType))
                {
                    throw new InvalidCastException(string.Format(Resources.Value.NotSubclassOfDisplayType.Value, body.GetType().FullName, nameof(displayType), displayType.FullName));
                }

                Body = body;
                Hide = hide;
                DisplayType = displayType;
            }
        }
    }
}
