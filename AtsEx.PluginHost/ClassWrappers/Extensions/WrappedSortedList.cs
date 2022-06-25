using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// 値がオリジナル型の <see cref="SortedList{TKey, TValue}"/> のラッパーを表します。
    /// </summary>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <typeparam name="TValueWrapper">値のオリジナル型に対応するラッパー型。</typeparam>
    /// <seealso cref="SortedList{TKey, TValue}"/>
    public class WrappedSortedList<TKey, TValueWrapper> : IDictionary<TKey, TValueWrapper>, IDictionary, IReadOnlyDictionary<TKey, TValueWrapper>
    {
        protected static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(WrappedSortedList<,>), @"PluginHost\ClassWrappers");

        protected IDictionary Src;
        protected dynamic SrcAsDynamic;
        protected ITwoWayConverter<object, TValueWrapper> ValueConverter;

        protected Type SrcType;

        protected FieldInfo VersionField;
        protected FieldInfo KeysField;
        protected FieldInfo ValuesField;

        protected int Version => (int)VersionField.GetValue(Src);
        protected TKey[] KeyArray => KeysField.GetValue(Src) as TKey[];
        protected Array ValueArray => ValuesField.GetValue(Src) as Array;

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <param name="parserToWrapper">オリジナル型からラッパー型に変換するためのデリゲート。</param>
        /// <param name="parserToSource">ラッパー型からオリジナル型に変換するためのデリゲート。</param>
        public WrappedSortedList(IDictionary src, ITwoWayConverter<object, TValueWrapper> valueConverter)
        {
            Src = src;
            SrcAsDynamic = Src;

            SrcType = Src.GetType();

            VersionField = SrcType.GetField("version", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
            KeysField = SrcType.GetField("keys", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
            ValuesField = SrcType.GetField("values", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <param name="parserToWrapper">オリジナル型からラッパー型に変換するためのデリゲート。</param>
        public WrappedSortedList(IDictionary src, Converter<object, TValueWrapper> converterToWrapper) : this(src, new ConverterToWrapper(converterToWrapper))
        {
#if DEBUG
            if (!typeof(TValueWrapper).IsSubclassOf(typeof(ClassWrapperBase)))
            {
                throw new ArgumentException(string.Format(Resources.GetString("CannotOmitParameter").Value, nameof(TValueWrapper), nameof(ClassWrapperBase)));
            }
#endif
        }


        public TValueWrapper this[TKey key]
        {
            get => ValueConverter.Convert(Src[key]);
            set => Src[key] = ValueConverter.ConvertBack(value);
        }

        object IDictionary.this[object key]
        {
            get => this[(TKey)key];
            set => this[(TKey)key] = (TValueWrapper)value;
        }

        public int Capacity
        {
            get => KeyArray.Length;
            set => SrcAsDynamic.Capacity = value;
        }

        public Comparer<TValueWrapper> Comparer => SrcAsDynamic.Comparer;

        public int Count => Src.Count;

        public bool IsFixedSize => Src.IsFixedSize;

        public bool IsReadOnly => Src.IsReadOnly;

        public bool IsSynchronized => Src.IsSynchronized;

        public ICollection<TKey> Keys => Src.Keys as ICollection<TKey>;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValueWrapper>.Keys => Src.Keys as IEnumerable<TKey>;

        ICollection IDictionary.Keys => Src.Keys;

        public object SyncRoot => Src.SyncRoot;

        protected WrappedValueList __Values;
#pragma warning disable IDE1006 // 命名スタイル
        protected WrappedValueList _Values => __Values = __Values ?? new WrappedValueList(this);
#pragma warning restore IDE1006 // 命名スタイル

        public ICollection<TValueWrapper> Values => _Values;

        IEnumerable<TValueWrapper> IReadOnlyDictionary<TKey, TValueWrapper>.Values => _Values;

        ICollection IDictionary.Values => _Values;


        public void Add(TKey key, TValueWrapper value) => Src.Add(key, ValueConverter.ConvertBack(value));

        void ICollection<KeyValuePair<TKey, TValueWrapper>>.Add(KeyValuePair<TKey, TValueWrapper> item) => Add(item.Key, item.Value);

        void IDictionary.Add(object key, object value) => Add((TKey)key, (TValueWrapper)value);

        public void Clear() => Src.Clear();

        public bool Contains(KeyValuePair<TKey, TValueWrapper> item)
        {
            int index = IndexOfKey(item.Key);
            return index >= 0 && Count > index && ValueArray.GetValue(index) == ValueConverter.ConvertBack(item.Value);
        }

        public bool ContainsKey(TKey key) => Src.Contains(key);

        bool IDictionary.Contains(object key) => ContainsKey((TKey)key);

        public bool ContainsValue(TValueWrapper value)
        {
            int index = IndexOfValue(value);
            return 0 <= index && index < Count;
        }

        public void CopyTo(KeyValuePair<TKey, TValueWrapper>[] array, int arrayIndex)
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

        void ICollection.CopyTo(Array array, int index) => CopyTo(array as KeyValuePair<TKey, TValueWrapper>[], index);

        public IEnumerator<KeyValuePair<TKey, TValueWrapper>> GetEnumerator() => new Enumerator(this, EnumeratorType.Enumerator);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this, EnumeratorType.Enumerator);

        IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(this, EnumeratorType.DictionaryEnumerator);

        public int IndexOfKey(TKey key) => Array.IndexOf(KeyArray, key);

        public int IndexOfValue(TValueWrapper value) => Array.IndexOf(ValueArray, ValueConverter.ConvertBack(value));

        public bool Remove(TKey key)
        {
            try
            {
                Src.Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValueWrapper> item)
        {
            if (Contains(item))
            {
                Remove(item.Key);
                return true;
            }
            else
            {
                return false;
            }
        }

        void IDictionary.Remove(object key) => Remove((TKey)key);

        public void RemoveAt(int index) => SrcAsDynamic.RemoveAt(index);

        public void TrimExcess() => SrcAsDynamic.TrimExcess();

        public bool TryGetValue(TKey key, out TValueWrapper value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        protected sealed class ConverterToWrapper : ITwoWayConverter<object, TValueWrapper>
        {
            private readonly Converter<object, TValueWrapper> Converter;

            public ConverterToWrapper(Converter<object, TValueWrapper> converter)
            {
                Converter = converter;
            }

            public TValueWrapper Convert(object value) => Converter(value);
            public object ConvertBack(TValueWrapper value) => (value as ClassWrapperBase).Src;
        }

        protected sealed class WrappedValueList : ICollection<TValueWrapper>, ICollection
        {
            private readonly WrappedSortedList<TKey, TValueWrapper> SortedList;

            public WrappedValueList(WrappedSortedList<TKey, TValueWrapper> sortedList)
            {
                if (sortedList is null)
                {
                    throw new ArgumentNullException();
                }

                SortedList = sortedList;
            }

            public int Count => SortedList.Count;

            public bool IsReadOnly => true;

            public object SyncRoot => SortedList.SyncRoot;

            public bool IsSynchronized => false;

            public void Add(TValueWrapper item) => throw new NotSupportedException();

            public void Clear() => throw new NotSupportedException();

            public bool Contains(TValueWrapper item) => SortedList.ContainsValue(item);

            public void CopyTo(TValueWrapper[] array, int arrayIndex)
            {
                if (!(array is null) && array.Rank != 1)
                {
                    throw new ArgumentException();
                }

                try
                {
                    Array.Copy(SortedList.Values.ToArray(), 0, array, arrayIndex, SortedList.Count);
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException();
                }
            }

            public void CopyTo(Array array, int index) => CopyTo(array as TValueWrapper[], index);

            public IEnumerator<TValueWrapper> GetEnumerator() => new WrappedValueEnumerator(SortedList);

            public bool Remove(TValueWrapper item) => throw new NotSupportedException();

            IEnumerator IEnumerable.GetEnumerator() => new WrappedValueEnumerator(SortedList);
        }

        protected enum EnumeratorType
        {
            Enumerator,
            DictionaryEnumerator,
        }

        protected sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValueWrapper>>, IDisposable, IEnumerator, IDictionaryEnumerator
        {
            private readonly WrappedSortedList<TKey, TValueWrapper> SortedList;
            private readonly EnumeratorType Type;
            
            private readonly IEnumerator<TKey> KeyEnumerator;
            private readonly IEnumerator<TValueWrapper> ValueEnumerator;

            public Enumerator(WrappedSortedList<TKey, TValueWrapper> sortedList, EnumeratorType type)
            {
                SortedList = sortedList;
                Type = type;

                KeyEnumerator = SortedList.Keys.GetEnumerator();
                ValueEnumerator = SortedList.Values.GetEnumerator();
            }

            public KeyValuePair<TKey, TValueWrapper> Current => new KeyValuePair<TKey, TValueWrapper>(KeyEnumerator.Current, ValueEnumerator.Current);

            public object Key => KeyEnumerator.Current;

            public object Value => ValueEnumerator.Current;

            public DictionaryEntry Entry => new DictionaryEntry(KeyEnumerator.Current, ValueEnumerator.Current);

            object IEnumerator.Current
            {
                get
                {
                    switch (Type)
                    {
                        case EnumeratorType.Enumerator:
                            return Current;
                        case EnumeratorType.DictionaryEnumerator:
                            return Entry;
                        default:
                            throw new NotSupportedException(string.Format(Resources.GetString("TypeNotSupported").Value, nameof(Type), Type));
                    }
                }
            }

            public void Dispose()
            {
                KeyEnumerator.Dispose();
                ValueEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                bool keyEnumeratorResult = KeyEnumerator.MoveNext();
                bool valueEnumeratorResult = ValueEnumerator.MoveNext();

                return keyEnumeratorResult != valueEnumeratorResult ? throw new AccessViolationException() : keyEnumeratorResult;
            }

            public void Reset()
            {
                KeyEnumerator.Reset();
                ValueEnumerator.Reset();
            }
        }

        protected sealed class WrappedValueEnumerator : IEnumerator<TValueWrapper>, IDisposable, IEnumerator
        {
            private readonly WrappedSortedList<TKey, TValueWrapper> SortedList;
            private readonly int Version;

            private int Index;

            public WrappedValueEnumerator(WrappedSortedList<TKey, TValueWrapper> sortedList)
            {
                SortedList = sortedList;
                Version = SortedList.Version;
            }

            public TValueWrapper Current { get; private set; }

            object IEnumerator.Current => Index == 0 || Index == SortedList.Count + 1 ? throw new InvalidOperationException() : Current;

            public void Dispose()
            {
                Index = 0;
                Current = default;
            }

            public bool MoveNext()
            {
                if (Version != SortedList.Version)
                {
                    throw new InvalidOperationException();
                }

                if (Index < SortedList.Count)
                {
                    object currentValueSrc = SortedList.ValueArray.GetValue(Index);
                    Current = SortedList.ValueConverter.Convert(currentValueSrc);
                    Index++;
                    return true;
                }

                Index = SortedList.Count + 1;
                Current = default;
                return false;
            }

            public void Reset()
            {
                if (Version != SortedList.Version)
                {
                    throw new InvalidOperationException();
                }

                Index = 0;
                Current = default;
            }
        }
    }
}
