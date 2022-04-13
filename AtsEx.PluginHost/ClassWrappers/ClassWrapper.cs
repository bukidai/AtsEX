using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class ClassWrapper
    {
        public dynamic Src { get; }

        public ClassWrapper(dynamic src)
        {
            Src = src;
        }

        public override bool Equals(object obj)
        {
            if (obj is ClassWrapper classWrapper)
            {
                return (Src as object).Equals(classWrapper.Src);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (Src as object).GetHashCode();
        }

        public override string ToString()
        {
            return (Src as object).ToString();
        }
    }
}