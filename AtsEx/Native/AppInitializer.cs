using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost;

namespace AtsEx.Native
{
    internal static class AppInitializer
    {
        public static void Initialize(CallerInfo callerInfo, LaunchMode launchMode)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            App.CreateInstance(callerInfo.Process, callerInfo.BveAssembly, callerInfo.AtsExLauncherAssembly, executingAssembly, launchMode);
        }
    }
}
