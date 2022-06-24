using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypeCollection;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// <see cref="MapObjectBase"/> のリストを表します。
    /// </summary>
    /// <seealso cref="SingleStructureList"/>
    /// <seealso cref="StationList"/>
    public class MapObjectList : ClassWrapperBase, IList<MapObjectBase>
    {
        protected static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<MapObjectList>(@"PluginHost\ClassWrappers");

        [InitializeClassWrapper]
        private static void Initialize()
        {
            ClassMemberCollection members = BveTypeCollectionProvider.Instance.GetClassInfoOf<MapObjectList>();
        }

        protected MapObjectList(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="MapObjectList"/> クラスのインスタンス。</returns>
        public static MapObjectList FromSource(object src)
        {
            if (src is null) return null;
            return new MapObjectList(src);
        }

        [UnderConstruction]
        private static MapObjectBase Parse(object src)
        {
            Type originalType = src.GetType();
            Type wrapperType = BveTypeCollectionProvider.Instance.GetWrapperTypeOf(originalType);

            if (wrapperType == typeof(PutBetweenStructure)) return PutBetweenStructure.FromSource(src);
            else if (wrapperType == typeof(Station)) return Station.FromSource(src);
            else if (wrapperType == typeof(Structure)) return Structure.FromSource(src);
            else
            {
                throw new DevelopException(string.Format(Resources.GetString("MapObjectTypeNotRecognized").Value, nameof(MapObjectBase), wrapperType.Name));
            }
        }

        public MapObjectBase this[int index]
        {
            get => Parse(Src[index]);
            set => Src[index] = value.Src;
        }

        public int Count => Src.Count;

        public bool IsReadOnly => Src.IsReadOnly;

        public virtual void Add(MapObjectBase item) => Src.Add(item.Src);

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
