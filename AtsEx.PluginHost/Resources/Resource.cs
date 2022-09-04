using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.Resources
{
    public class Resource<T>
    {
        public CultureInfo Culture { get; }
        public T Value { get; }

        public Resource(CultureInfo culture, T value)
        {
            Culture = culture;
            Value = value;
        }
    }
}
