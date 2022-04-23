using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    internal class ClassMemberNameCollection : TypeMemberNameCollectionBase
    {
        public List<PropertyAccessorInfo> PropertyGetters { get; }
        public List<PropertyAccessorInfo> PropertySetters { get; }
        public List<FieldInfo> Fields { get; }
        public List<MethodInfo> Methods { get; }

        public ClassMemberNameCollection(string wrapperTypeName, string originalTypeName, IEnumerable<TypeMemberNameCollectionBase> children) : base(wrapperTypeName, originalTypeName, children)
        {
            PropertyGetters = new List<PropertyAccessorInfo>();
            PropertySetters = new List<PropertyAccessorInfo>();
            Fields = new List<FieldInfo>();
            Methods = new List<MethodInfo>();
        }
    }
}
