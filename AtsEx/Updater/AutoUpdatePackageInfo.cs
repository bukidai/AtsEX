using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Automatic9045.AtsEx.Export
{
    internal class AutoUpdatePackageInfo
    {
        public bool IncludesMainAssembly { get; protected set; } = false;

        protected AutoUpdatePackageInfo()
        {
        }

        public static AutoUpdatePackageInfo Default => new AutoUpdatePackageInfo();

        public static AutoUpdatePackageInfo FromXDocument(XDocument document)
        {
            XElement rootElement = document.Element("AtsExAutoUpdatePackageConfig");
            if (rootElement is null) throw new FormatException("ルート要素 'AtsExAutoUpdatePackageConfig' が存在しません。");

            AutoUpdatePackageInfo config = new AutoUpdatePackageInfo();

            XElement includeMainAssemblyElement = rootElement.Element("IncludesMainAssembly");
            config.IncludesMainAssembly = (bool?)includeMainAssemblyElement ?? false;

            return config;
        }
    }
}
