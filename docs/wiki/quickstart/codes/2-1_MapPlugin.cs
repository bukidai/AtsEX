using System;
using AtsEx.PluginHost.Plugins;

[PluginType(PluginType.MapPlugin)]
internal class PluginMain : AssemblyPluginBase
{
    public PluginMain(PluginBuilder builder) : base(builder)
    {
    }

    public override void Dispose()
    {
    }

    public override void Tick(TimeSpan elapsed)
    {
        return new MapPluginTickResult();
    }
}