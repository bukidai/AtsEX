using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost;
using AtsEx.PluginHost.Handles;
using AtsEx.PluginHost.Plugins;

namespace AtsEx.Samples.VehiclePlugins.SimpleAts
{
    public class SimpleAts : AssemblyPluginBase
    {
        public SimpleAts(PluginBuilder builder) : base(builder, PluginType.VehiclePlugin)
        {
        }

        public override void Dispose()
        {
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            UserVehicleLocationManager locationManager = BveHacker.Scenario.LocationManager;
            PluginHost.Handles.HandleSet handleSet = Native.Handles;

            double speedMps = locationManager.SpeedMeterPerSecond;

            VehiclePluginTickResult tickResult = new VehiclePluginTickResult();
            if (speedMps > 100d.KmphToMps()) // 100km/h以上出ていたら常用最大ブレーキ
            {
                int atsPowerNotch = 0;
                int atsBrakeNotch = handleSet.Brake.MaxServiceBrakeNotch;

                NotchCommandBase powerCommand = handleSet.Power.GetCommandToSetNotchTo(atsPowerNotch);
                NotchCommandBase brakeCommand = handleSet.Brake.GetCommandToSetNotchTo(Math.Max(atsBrakeNotch, handleSet.Brake.Notch));
                ReverserPositionCommandBase reverserCommand = ReverserPositionCommandBase.Continue;
                ConstantSpeedCommand? constantSpeedCommand = ConstantSpeedCommand.Disable;

                tickResult.HandleCommandSet = new HandleCommandSet(powerCommand, brakeCommand, reverserCommand, constantSpeedCommand);
            }

            return tickResult;
        }
    }
}
