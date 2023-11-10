using System;
using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

[PluginType(PluginType.Extension)]
[HideExtensionMain]
internal class 任意のクラス名 : AssemblyPluginBase, IExtension
{
    // ...略...
}