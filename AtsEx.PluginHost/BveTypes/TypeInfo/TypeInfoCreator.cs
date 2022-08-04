using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.BveTypes
{
    internal class TypeInfoCreator
    {
        private readonly Assembly BveAssembly;
        private readonly Assembly AtsExPluginHostAssembly = Assembly.GetExecutingAssembly();

        private readonly IEnumerable<Type> WrapperTypes;
        private readonly IEnumerable<Type> OriginalTypes;

        public TypeInfoCreator(Assembly bveAssembly)
        {
            BveAssembly = bveAssembly;

            WrapperTypes = AtsExPluginHostAssembly.GetTypes().Where(type => (type.IsClass && type.IsSubclassOf(typeof(ClassWrapperBase))) || type.IsEnum);
            OriginalTypes = BveAssembly.GetTypes();
        }

        public List<TypeInfo> Create(List<TypeMemberNameSetBase> src)
        {
            List<TypeInfo> typeInfos = new List<TypeInfo>();

            src.ForEach(item =>
            {
                IEnumerable<TypeInfo> children = Create(item.Children);
                typeInfos.AddRange(children);

                TypeInfo typeInfo = Create(item);
                typeInfos.Add(typeInfo);
            });

            return typeInfos;
        }

        public TypeInfo Create(TypeMemberNameSetBase src)
        {
            Type wrapperType = WrapperTypes.FirstOrDefault(t => t.Name == src.WrapperTypeName);

            TypeInfo parentTypeInfo = null;
            Type originalType;
            if (src.Parent is null)
            {
                originalType = OriginalTypes.FirstOrDefault(t => t.DeclaringType is null && t.FullName == src.OriginalTypeName);
            }
            else
            {
                parentTypeInfo = Create(src.Parent);
                originalType = OriginalTypes.FirstOrDefault(t => t.DeclaringType == parentTypeInfo.OriginalType && t.Name == src.OriginalTypeName);
            }

            IEnumerable<TypeInfo> children = src.Children.Select(Create);

            TypeInfo typeInfo = new TypeInfo(wrapperType, originalType, parentTypeInfo, children, src);
            return typeInfo;
        }
    }
}
