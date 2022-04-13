﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public class MapObjectList : ClassWrapper, IList<MapObjectBase>
    {
        static MapObjectList()
        {
            BveTypeMemberCollection members = BveTypeCollectionProvider.Instance.GetTypeInfoOf<MapObjectList>();
        }

        protected MapObjectList(object src) : base(src)
        {
        }

        public static MapObjectList FromSource(object src) => new MapObjectList(src);

        [UnderConstruction]
        private static MapObjectBase Parse(object src)
        {
            Type originalType = src.GetType();
            Type wrapperType = BveTypeCollectionProvider.Instance.GetWrapperTypeOf(originalType);
            if (wrapperType == typeof(Station))
            {
                return Station.FromSource(src);
            }
            else
            {
                throw new DevelopException($"{nameof(MapObjectBase)} '{wrapperType.Name}' は認識されていません。");
            }
        }

        public MapObjectBase this[int index]
        {
            get => Parse(Src[index]);
            set => Src[index] = value.Src;
        }

        public int Count => Src.Count;

        public bool IsReadOnly => Src.IsReadOnly;

        public void Add(MapObjectBase item) => Src.Add(item.Src);

        public void Clear() => Src.Clear();

        public bool Contains(MapObjectBase item) => Src.Contains(item.Src);

        [UnderConstruction]
        public void CopyTo(MapObjectBase[] array, int arrayIndex) => Src.ConvertAll(new Converter<dynamic, MapObjectBase>(Parse)).CopyTo(array, arrayIndex);

        public IEnumerator<MapObjectBase> GetEnumerator() => new Enumerator(Src.GetEnumerator());

        public int IndexOf(MapObjectBase item) => Src.IndexOf(item.Src);

        public void Insert(int index, MapObjectBase item) => Src.Insert(index, item.Src);

        public bool Remove(MapObjectBase item) => Src.Remove(item.Src);

        public void RemoveAt(int index) => Src.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal sealed class Enumerator : IEnumerator<MapObjectBase>, IEnumerator
        {
            public IEnumerator Src { get; }

            public Enumerator(IEnumerator src)
            {
                Src = src;
            }

            MapObjectBase IEnumerator<MapObjectBase>.Current => Parse(Src.Current);

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
