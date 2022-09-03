using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// オリジナル型の <see cref="List{T}"/> のラッパーを表します。
    /// </summary>
    /// <typeparam name="TWrapper">値のオリジナル型に対応するラッパー型。</typeparam>
    /// <seealso cref="SortedList{TKey, TValue}"/>
    public class WrappedList<TWrapper> : ClassWrapperBase, IList<TWrapper>, IReadOnlyList<TWrapper>, IList
    {
        private static BveTypeSet BveTypes = null;
        private static ListConstructorSet ListConstructors = null;

        protected readonly new IList Src;
        protected readonly ITwoWayConverter<object, TWrapper> Converter;


        private static void LoadConstructors()
        {
            Type originalType = BveTypes.GetTypeInfoOf<TWrapper>().OriginalType;
            Type listType = typeof(List<>).MakeGenericType(originalType);
            ListConstructors = new ListConstructorSet(listType);
        }


        protected WrappedList(IList src, ITwoWayConverter<object, TWrapper> converter) : base(src)
        {
            if (BveTypes is null)
            {
                BveTypes = ClassWrapperInitializer.LazyInitialize();
                LoadConstructors();
            }

            Src = src;
            if (!IsSubclassOfGeneric(Src.GetType(), typeof(List<>))) throw new ArgumentException();

            Converter = converter;


            bool IsSubclassOfGeneric(Type current, Type genericBase)
            {
                do
                {
                    if (current.IsGenericType && current.GetGenericTypeDefinition() == genericBase) return true;
                }
                while ((current = current.BaseType) != null);
                return false;
            }
        }

        protected WrappedList(IList src) : this(src, new ClassWrapperConverter<TWrapper>())
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <param name="converter">オリジナル型とラッパー型を相互に変換するための <see cref="ITwoWayConverter{T1, T2}"/>。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="WrappedList{TWrapper}"/> クラスのインスタンス。</returns>
        public static WrappedList<TWrapper> FromSource(IList src, ITwoWayConverter<object, TWrapper> converter) => src is null ? null : new WrappedList<TWrapper>(src, converter);

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="WrappedList{TWrapper}"/> クラスのインスタンス。</returns>
        public static WrappedList<TWrapper> FromSource(IList src) => FromSource(src, new ClassWrapperConverter<TWrapper>());


        public WrappedList() : base(ListConstructors.Create())
        {
        }

        public WrappedList(IEnumerable collection) : base(ListConstructors.Create(collection))
        {
        }

        public WrappedList(int capacity) : base(ListConstructors.Create(capacity))
        {
        }


        /// <inheritdoc/>
        public TWrapper this[int index]
        {
            get => Converter.Convert(Src[index]);
            set => Src[index] = Converter.ConvertBack(value);
        }

        /// <inheritdoc/>
        public int Count => Src.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => Src.IsReadOnly;

        bool IList.IsFixedSize => Src.IsFixedSize;
        object ICollection.SyncRoot => Src.SyncRoot;
        bool ICollection.IsSynchronized => Src.IsSynchronized;

        /// <inheritdoc/>
        public virtual void Add(TWrapper item) => Src.Add(Converter.ConvertBack(item));

        /// <inheritdoc/>
        public void Clear() => Src.Clear();

        /// <inheritdoc/>
        public bool Contains(TWrapper item) => Src.Contains(Converter.ConvertBack(item));

        /// <inheritdoc/>
        public void CopyTo(TWrapper[] array, int arrayIndex)
        {
            if (!(array is null) && array.Rank != 1)
            {
                throw new ArgumentException();
            }

            try
            {
                Array.Copy(this.ToArray(), 0, array, arrayIndex, Count);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException();
            }
        }

        /// <inheritdoc/>
        public IEnumerator<TWrapper> GetEnumerator() => new Enumerator(Src.GetEnumerator(), Converter);

        /// <inheritdoc/>
        public int IndexOf(TWrapper item) => Src.IndexOf(Converter.ConvertBack(item));

        /// <inheritdoc/>
        public void Insert(int index, TWrapper item) => Src.Insert(index, Converter.ConvertBack(item));

        /// <inheritdoc/>
        public bool Remove(TWrapper item)
        {
            object original = Converter.ConvertBack(item);
            if (Src.Contains(original))
            {
                Src.Remove(original);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void RemoveAt(int index) => Src.RemoveAt(index);

        object IList.this[int index] { get => this[index]; set => this[index] = (TWrapper)value; }
        int IList.Add(object value)
        {
            Add((TWrapper)value);
            return Count - 1;
        }
        bool IList.Contains(object value) => Contains((TWrapper)value);
        void ICollection.CopyTo(Array array, int index) => CopyTo((TWrapper[])array, index);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        int IList.IndexOf(object value) => IndexOf((TWrapper)value);
        void IList.Insert(int index, object value) => Insert(index, (TWrapper)value);
        void IList.Remove(object value) => Remove((TWrapper)value);

        internal sealed class ListConstructorSet
        {
            private readonly ConstructorInfo Default;

            private readonly ConstructorInfo WithItems;

            private readonly ConstructorInfo WithCapacitySpecified;
            private static readonly Type[] WithCapacitySpecifiedParameters = new Type[] { typeof(int) };

            public ListConstructorSet(Type type)
            {
                Default = type.GetConstructor(Type.EmptyTypes);
                WithItems = type.GetConstructor(new Type[] { typeof(IEnumerable<>).MakeGenericType(new Type[] { type.GenericTypeArguments[0] }) });
                WithCapacitySpecified = type.GetConstructor(WithCapacitySpecifiedParameters);
            }

            public object Create() => Default.Invoke(null);
            public object Create(IEnumerable collection) => WithItems.Invoke(new object[] { collection });
            public object Create(int capacity) => WithCapacitySpecified.Invoke(new object[] { capacity });
        }

        internal sealed class Enumerator : IEnumerator<TWrapper>, IEnumerator
        {
            private readonly IEnumerator Src;
            private readonly ITwoWayConverter<object, TWrapper> Converter;

            public Enumerator(IEnumerator src, ITwoWayConverter<object, TWrapper> converter)
            {
                Src = src;
                Converter = converter;
            }

            public TWrapper Current => Converter.Convert(Src.Current);

            object IEnumerator.Current => Current;

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
