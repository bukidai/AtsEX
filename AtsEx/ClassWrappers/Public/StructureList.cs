using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost;
using Automatic9045.AtsEx.PluginHost.ClassWrappers;

namespace Automatic9045.AtsEx.ClassWrappers
{
    internal class StructureList : ClassWrapper, IStructureList
    {
        static StructureList()
        {
            IBveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<IStructureList>();
        }

        public StructureList(object src) : base(src)
        {
        }

        [UnderConstruction]
        private static IMapObjectBase Parse(object src)
        {
            switch (src)
            {
                default:
                    throw new NotImplementedException();
            }
        }

        public IMapObjectBase this[int index]
        {
            get => Parse(Src[index]);
            set => Src[index] = value.Src;
        }

        public int Count => Src.Count;

        public bool IsReadOnly => Src.IsReadOnly;

        public void Add(IMapObjectBase item) => Src.Add(item.Src);

        public void Clear() => Src.Clear();

        public bool Contains(IMapObjectBase item) => Src.Contains(item.Src);

        [UnderConstruction]
        public void CopyTo(IMapObjectBase[] array, int arrayIndex) => Src.ConvertAll(new Converter<dynamic, IMapObjectBase>(Parse)).CopyTo(array, arrayIndex);

        public IEnumerator<IMapObjectBase> GetEnumerator() => new Enumerator(Src.GetEnumerator());

        public int IndexOf(IMapObjectBase item) => Src.IndexOf(item.Src);

        public void Insert(int index, IMapObjectBase item) => Src.Insert(index, item.Src);

        public bool Remove(IMapObjectBase item) => Src.Remove(item.Src);

        public void RemoveAt(int index) => Src.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal sealed class Enumerator : IEnumerator<IMapObjectBase>, IEnumerator
        {
            public IEnumerator Src { get; }

            public Enumerator(IEnumerator src)
            {
                Src = src;
            }

            IMapObjectBase IEnumerator<IMapObjectBase>.Current => Parse(Src.Current);

            public object Current => Parse(Src.Current);

            public bool MoveNext() => Src.MoveNext();

            public void Reset() => Src.Reset();

            public void Dispose()
            {
                if (Src is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
