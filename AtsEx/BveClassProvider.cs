using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.CoreHackServices;
using Automatic9045.AtsEx.ClassWrappers;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    internal static class BveClassProvider
    {
        private const string BvetsNamespace = "Mackoy.Bvets";

        private static Dictionary<Type, string> _ClassNames = new Dictionary<Type, string>()
        {
            { typeof(ScenarioInfo), $"ek" },
        };
        public static ReadOnlyDictionary<Type, string> ClassNames { get; } = new ReadOnlyDictionary<Type, string>(_ClassNames);
    }
}
