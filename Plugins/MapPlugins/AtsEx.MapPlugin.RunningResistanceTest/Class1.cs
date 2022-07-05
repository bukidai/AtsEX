using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.MapPlugins.RunningResistanceTest
{
    public class Class1 : AtsExPluginBase
    {
        private double defaultFactorA;
        private double defaultFactorB;
        private double defaultFactorC;

        public Class1(AtsExPluginBuilder builder) : base(builder, PluginType.MapPlugin)
        {
            BveHacker.ScenarioCreated += OnScenarioCreated;
        }

        public void Dispose()
        {
            BveHacker.ScenarioCreated -= OnScenarioCreated;
        }

        private void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            defaultFactorA = e.Scenario.Vehicle.Dynamics.RunningResistanceFactorA;
            defaultFactorB = e.Scenario.Vehicle.Dynamics.RunningResistanceFactorB;
            defaultFactorC = e.Scenario.Vehicle.Dynamics.RunningResistanceFactorC;
        }

        public override void Tick(TimeSpan elapsed)
        {
            double location = BveHacker.Scenario.LocationManager.Location;

            if (1550d < location && location < 1650d)
            {
                BveHacker.Scenario.Vehicle.Dynamics.RunningResistanceFactorA = 0d;
                BveHacker.Scenario.Vehicle.Dynamics.RunningResistanceFactorB = 50000d;
                BveHacker.Scenario.Vehicle.Dynamics.RunningResistanceFactorC = 0d;
            }
            else
            {
                BveHacker.Scenario.Vehicle.Dynamics.RunningResistanceFactorA = defaultFactorA;
                BveHacker.Scenario.Vehicle.Dynamics.RunningResistanceFactorB = defaultFactorB;
                BveHacker.Scenario.Vehicle.Dynamics.RunningResistanceFactorC = defaultFactorC;
            }
        }
    }
}
