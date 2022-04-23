using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.PluginHost.BveTypeCollection
{
    internal class TypeInfoGenerator
    {
        private Assembly BveAssembly;
        private Assembly AtsExAssembly;
        private Assembly AtsExPluginHostAssembly;

        private IEnumerable<Type> WrapperTypes;
        private IEnumerable<Type> OriginalTypes;

        public TypeInfoGenerator(Assembly bveAssembly, Assembly atsExAssembly)
        {
            BveAssembly = bveAssembly;
            AtsExAssembly = atsExAssembly;
            AtsExPluginHostAssembly = Assembly.GetExecutingAssembly();

            WrapperTypes = AtsExPluginHostAssembly.GetTypes().Concat(AtsExAssembly.GetTypes()).Where(type => (type.IsClass && type.IsSubclassOf(typeof(ClassWrapper))) || type.IsEnum);
            OriginalTypes = BveAssembly.GetTypes();
        }

        public IEnumerable<TypeInfo> ConvertTypeMemberNameCollections(IEnumerable<TypeMemberNameCollectionBase> src)
        {
            foreach (TypeMemberNameCollectionBase item in src)
            {
                IEnumerable<TypeInfo> children = ConvertTypeMemberNameCollections(item.Children);
                foreach (TypeInfo child in children) yield return child;

                TypeInfo typeInfo = ConvertTypeMemberNameCollection(item);
                yield return typeInfo;
            }
        }

        public TypeInfo ConvertTypeMemberNameCollection(TypeMemberNameCollectionBase src)
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
                parentTypeInfo = ConvertTypeMemberNameCollection(src.Parent);
                originalType = OriginalTypes.FirstOrDefault(t => t.DeclaringType == parentTypeInfo.OriginalType && t.Name == src.OriginalTypeName);
            }

            IEnumerable<TypeInfo> children = src.Children.Select(ConvertTypeMemberNameCollection);

            TypeInfo typeInfo = new TypeInfo(wrapperType, originalType, parentTypeInfo, children, src);
            return typeInfo;
        }
    }
}
