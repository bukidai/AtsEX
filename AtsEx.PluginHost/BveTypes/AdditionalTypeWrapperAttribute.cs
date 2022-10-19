using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.BveTypes
{
    internal class AdditionalTypeWrapperAttribute : Attribute
    {
        public Type Original { get; }

        public AdditionalTypeWrapperAttribute(Type original)
        {
            Original = original;
        }
    }
}
