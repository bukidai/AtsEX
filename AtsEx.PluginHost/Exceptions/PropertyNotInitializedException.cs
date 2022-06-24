using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.PluginHost
{
    public class PropertyNotInitializedException : Exception
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<PropertyNotInitializedException>("PluginHost");

        public PropertyNotInitializedException(string propertyName) : base(string.Format(Resources.GetString("Message").Value, propertyName))
        {
        }
    }
}
