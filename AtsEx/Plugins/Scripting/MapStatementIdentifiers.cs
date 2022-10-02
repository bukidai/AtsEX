using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnembeddedResources;

namespace Automatic9045.AtsEx.Plugins.Scripting
{
    internal static partial class MapStatementIdentifiers
    {
        private const string ScriptLanguagesBase = "AtsEx::ScriptLanguage.";
        public static readonly ReadOnlyDictionary<ScriptLanguage, string> ScriptLanguages = new ReadOnlyDictionary<ScriptLanguage, string>(new SortedList<ScriptLanguage, string>()
        {
            { ScriptLanguage.CSharpScript, (ScriptLanguagesBase + "CSharpScript").ToLower() },
            { ScriptLanguage.IronPython2, (ScriptLanguagesBase + "IronPython2").ToLower() },
        });

        internal static class ErrorTexts
        {
            private class ResourceSet
            {
                private readonly ResourceLocalizer Localizer = ResourceLocalizer.FromResXOfType(typeof(MapStatementIdentifiers), @"Core\Plugins");

                [ResourceStringHolder(nameof(Localizer))] public Resource<string> InvalidScriptLanguage { get; private set; }

                public ResourceSet()
                {
                    ResourceLoader.LoadAndSetAll(this);
                }
            }

            private static readonly ResourceSet Resources = new ResourceSet();

            public static string InvalidScriptLanguage(string invalidText) => string.Format(Resources.InvalidScriptLanguage.Value, invalidText);
        }
    }
}
