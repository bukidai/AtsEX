using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.BveTypeCollection
{
    public partial class BveTypeCollectionProvider : IBveTypeCollectionProvider
    {
        protected SortedList<Type, BveTypeMemberCollection> Types { get; }

        protected BveTypeCollectionProvider(SortedList<Type, BveTypeMemberCollection> types)
        {
            Type illegalType = types.Keys.FirstOrDefault(type => !type.IsInterface || !type.GetInterfaces().Contains(typeof(IClassWrapper)));
            if (!(illegalType is null))
            {
                throw new ArgumentException($"型 '{illegalType.FullName}' は {typeof(IClassWrapper).FullName} を継承したインターフェースではありません。");
            }

            Types = types;
        }

        public IBveTypeMemberCollection GetTypeInfoOf<TWrapper>() => Types[typeof(TWrapper)];

        public void Dispose()
        {
            Instance = null;
        }
    }
}
