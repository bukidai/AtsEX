﻿using System;
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
            private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(MapStatementIdentifiers), @"Core\Plugins");

            public static string InvalidScriptLanguage(string invalidText) => string.Format(Resources.GetString("InvalidScriptLanguage").Value, invalidText);
        }
    }
}
