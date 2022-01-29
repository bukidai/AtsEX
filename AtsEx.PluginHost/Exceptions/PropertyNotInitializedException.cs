using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    public class PropertyNotInitializedException : Exception
    {
        public PropertyNotInitializedException(string propertyName) : base($"プロパティ {propertyName} はまだ初期化されておらず、取得できません。")
        {
        }
    }
}
