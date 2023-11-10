using System;
using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

[PluginType(PluginType.Extension)]
[ExtensionMainDisplayType(typeof(IHogeExtension))]
internal class HogeExtension : AssemblyPluginBase, IHogeExtension
{
    // ...略...
}

public interface IHogeExtension : IExtension
{
}