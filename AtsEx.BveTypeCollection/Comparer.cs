using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx.BveTypeCollection
{
    internal class TypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            return x.FullName.CompareTo(y.FullName);
        }
    }

    internal class StringTypeArrayTupleComparer : IComparer<(string, Type[])>
    {
        public int Compare((string, Type[]) x, (string, Type[]) y)
        {
            string xText = x.Item1 + x.Item2.ToString();
            string yText = y.Item1 + y.Item2.ToString();
            return string.Compare(xText, yText);
        }
    }
}
