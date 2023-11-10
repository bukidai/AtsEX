using System;
using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

[PluginType(PluginType.Extension)]
internal class ExtensionMain : AssemblyPluginBase, IExtension
{
    public ExtensionMain(PluginBuilder builder) : base(builder)
    {
    }

    public override void Dispose()
    {
    }

    public override void Tick(TimeSpan elapsed)
    {
        return new ExtensionTickResult();
    }
}