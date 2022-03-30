using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx
{
    public class ClassWrapper : IClassWrapper
    {
        public dynamic Src { get; }

        public ClassWrapper(dynamic src)
        {
            Src = src;
        }
    }
}