using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    internal class ClassMemberNameSet : TypeMemberNameSetBase
    {
        public List<PropertyAccessor> PropertyGetters { get; }
        public List<PropertyAccessor> PropertySetters { get; }
        public List<Field> Fields { get; }
        public List<Method> Methods { get; }

        public ClassMemberNameSet(string wrapperTypeName, string originalTypeName, List<TypeMemberNameSetBase> children) : base(wrapperTypeName, originalTypeName, children)
        {
            PropertyGetters = new List<PropertyAccessor>();
            PropertySetters = new List<PropertyAccessor>();
            Fields = new List<Field>();
            Methods = new List<Method>();
        }
    }
}
