using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx
{
    internal class AssemblyResolver
    {
        public AppDomain TargetAppDomain { get; }

        public AssemblyResolver(AppDomain targetAppDomain)
        {
            TargetAppDomain = targetAppDomain;
        }

        public void Register(Assembly assembly)
        {
            TargetAppDomain.AssemblyResolve += (sender, e) =>
            {
                if (e.Name != assembly.FullName) return null;
#if DEBUG
                MessageBox.Show($"{assembly.Location} が登録されました。");
#endif
                return assembly;
            };
        }

        public void Register(string absolutePath)
        {
            Assembly assembly = Assembly.LoadFrom(absolutePath);
            Register(assembly);
        }
    }
}
