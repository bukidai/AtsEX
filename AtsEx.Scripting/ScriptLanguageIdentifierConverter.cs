using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

using AtsEx.PluginHost.MapStatements;

namespace AtsEx.Scripting
{
    public static partial class ScriptLanguageIdentifierConverter
    {
        private class ResourceSet
        {
            private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType(typeof(ScriptLanguageIdentifierConverter), @"Scripting");

            [ResourceStringHolder(nameof(Localizer))] public Resource<string> InvalidScriptLanguage { get; private set; }

            public ResourceSet()
            {
                ResourceLoader.LoadAndSetAll(this);
            }
        }

        private static readonly Lazy<ResourceSet> Resources = new Lazy<ResourceSet>();

        static ScriptLanguageIdentifierConverter()
        {
#if DEBUG
            _ = Resources.Value;
#endif
        }

        private static readonly Identifier RootIdentifier = new Identifier(Namespace.Root, "scriptlanguage");
        private static readonly IdentifierToEnumConverter<ScriptLanguage> Converter = new IdentifierToEnumConverter<ScriptLanguage>(RootIdentifier);

        public static bool TryConvert(Identifier value, out ScriptLanguage result) => Converter.TryConvert(value, out result);

        public static ScriptLanguage Convert(Identifier value)
            => TryConvert(value, out ScriptLanguage result) ? result : throw new ArgumentException(string.Format(Resources.Value.InvalidScriptLanguage.Value, value.FullName));
    }
}
