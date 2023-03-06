using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtsEx.PluginHost.MapStatements;

namespace AtsEx.Samples.MapPlugins.MapStatementTest
{
    internal static class Identifiers
    {
        private static readonly Namespace Namespace = Namespace.GetUserNamespace("Automatic9045").Child("Alert");

        public static readonly Identifier HeaderAlert = new Identifier(Namespace, "HeaderAlert");

        public static class Alert
        {
            private static readonly Identifier Root = new Identifier(Namespace, "Alert");

            public static readonly Identifier Put = new Identifier(Root, "Put");
            public static readonly Identifier PutRange = new Identifier(Root, "PutRange");
        }

        public static class AlertType
        {
            private static readonly Identifier Root = new Identifier(Namespace, "AlertType");

            public static readonly Identifier Basic = new Identifier(Root, "Basic");
            public static readonly Identifier ShowValue = new Identifier(Root, "ShowValue");
        }
    }
}
