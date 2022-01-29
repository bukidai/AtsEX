using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    [UnderConstruction]
    public class BveFile : ClassWrapper, IBveFile
    {
        public BveFile(Assembly assembly, object src) : base(src)
        {
            PathGetMethod = GetMethod("d");
            PathSetMethod = GetMethod("c", typeof(string));
        }

        protected MethodInfo PathGetMethod;
        protected MethodInfo PathSetMethod;
        public string Path
        {
            get => PathGetMethod.Invoke(Src, null);
            internal set => PathSetMethod.Invoke(Src, new object[] { value });
        }
    }
}
