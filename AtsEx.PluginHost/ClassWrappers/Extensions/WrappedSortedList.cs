﻿using System;
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
    /// 値がオリジナル型の <see cref="SortedList{TKey, TValue}"/> のラッパーを表します。
    /// </summary>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <typeparam name="TValueWrapper">値のオリジナル型に対応するラッパー型。</typeparam>
    /// <seealso cref="SortedList{TKey, TValue}"/>
    public class WrappedSortedList<TKey, TValueWrapper> : IDictionary<TKey, TValueWrapper>, IDictionary, IReadOnlyDictionary<TKey, TValueWrapper>
    {
        protected static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType(typeof(WrappedSortedList<,>), @"PluginHost\ClassWrappers");

        private static BveTypeSet BveTypes = null;

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
        /// <param name="valueConverter">オリジナル型とラッパー型を相互に変換するための <see cref="ITwoWayConverter{T1, T2}"/>。</param>
        public WrappedSortedList(IDictionary src, ITwoWayConverter<object, TValueWrapper> valueConverter)
        {
            if (BveTypes is null)
            {
                BveTypes = ClassWrapperInitializer.LazyInitialize();
            }

            Src = src;
            SrcAsDynamic = Src;
            ValueConverter = valueConverter;

            SrcType = Src.GetType();

            VersionField = SrcType.GetField("version", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
            KeysField = SrcType.GetField("keys", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
            ValuesField = SrcType.GetField("values", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        public WrappedSortedList(IDictionary src) : this(src, new ClassWrapperConverter<TValueWrapper>())
        {
#if DEBUG
            if (!typeof(TValueWrapper).IsSubclassOf(typeof(ClassWrapperBase)))
            {
                throw new ArgumentException(string.Format(Resources.GetString("CannotOmitParameter").Value, nameof(TValueWrapper), nameof(ClassWrapperBase)));
            }
#endif
        }


        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public int Capacity
        {
            get => KeyArray.Length;
            set => SrcAsDynamic.Capacity = value;
        }

        /// <inheritdoc/>
        public Comparer<TValueWrapper> Comparer => SrcAsDynamic.Comparer;

        /// <inheritdoc/>
        public int Count => Src.Count;

        /// <inheritdoc/>
        public bool IsFixedSize => Src.IsFixedSize;

        /// <inheritdoc/>
        public bool IsReadOnly => Src.IsReadOnly;

        /// <inheritdoc/>
        public bool IsSynchronized => Src.IsSynchronized;

        /// <inheritdoc/>
        public ICollection<TKey> Keys => Src.Keys as ICollection<TKey>;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValueWrapper>.Keys => Src.Keys as IEnumerable<TKey>;

        ICollection IDictionary.Keys => Src.Keys;

        /// <inheritdoc/>
        public object SyncRoot => Src.SyncRoot;

        protected WrappedValueList __Values;
#pragma warning disable IDE1006 // 命名スタイル
        protected WrappedValueList _Values => __Values = __Values ?? new WrappedValueList(this);
#pragma warning restore IDE1006 // 命名スタイル

        /// <inheritdoc/>
        public ICollection<TValueWrapper> Values => _Values;

        IEnumerable<TValueWrapper> IReadOnlyDictionary<TKey, TValueWrapper>.Values => _Values;

        ICollection IDictionary.Values => _Values;



        /// <inheritdoc/>
        public void Add(TKey key, TValueWrapper value) => Src.Add(key, ValueConverter.ConvertBack(value));

        void ICollection<KeyValuePair<TKey, TValueWrapper>>.Add(KeyValuePair<TKey, TValueWrapper> item) => Add(item.Key, item.Value);

        void IDictionary.Add(object key, object value) => Add((TKey)key, (TValueWrapper)value);

        /// <inheritdoc/>
        public void Clear() => Src.Clear();

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValueWrapper> item)
        {
            int index = IndexOfKey(item.Key);
            return index >= 0 && Count > index && ValueArray.GetValue(index) == ValueConverter.ConvertBack(item.Value);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key) => Src.Contains(key);

        bool IDictionary.Contains(object key) => ContainsKey((TKey)key);

        /// <inheritdoc/>
        public bool ContainsValue(TValueWrapper value)
        {
            int index = IndexOfValue(value);
            return 0 <= index && index < Count;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValueWrapper>> GetEnumerator() => new Enumerator(this, EnumeratorType.Enumerator);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this, EnumeratorType.Enumerator);

        IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(this, EnumeratorType.DictionaryEnumerator);

        /// <inheritdoc/>
        public int IndexOfKey(TKey key) => Array.IndexOf(KeyArray, key);

        /// <inheritdoc/>
        public int IndexOfValue(TValueWrapper value) => Array.IndexOf(ValueArray, ValueConverter.ConvertBack(value));

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void RemoveAt(int index) => SrcAsDynamic.RemoveAt(index);

        /// <inheritdoc/>
        public void TrimExcess() => SrcAsDynamic.TrimExcess();

        /// <inheritdoc/>
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

            /// <inheritdoc/>
            public int Count => SortedList.Count;

            /// <inheritdoc/>
            public bool IsReadOnly => true;

            /// <inheritdoc/>
            public object SyncRoot => SortedList.SyncRoot;

            /// <inheritdoc/>
            public bool IsSynchronized => false;

            /// <inheritdoc/>
            public void Add(TValueWrapper item) => throw new NotSupportedException();

            /// <inheritdoc/>
            public void Clear() => throw new NotSupportedException();

            /// <inheritdoc/>
            public bool Contains(TValueWrapper item) => SortedList.ContainsValue(item);

            /// <inheritdoc/>
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

            /// <inheritdoc/>
            public void CopyTo(Array array, int index) => CopyTo(array as TValueWrapper[], index);

            /// <inheritdoc/>
            public IEnumerator<TValueWrapper> GetEnumerator() => new WrappedValueEnumerator(SortedList);

            /// <inheritdoc/>
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

            /// <inheritdoc/>
            public KeyValuePair<TKey, TValueWrapper> Current => new KeyValuePair<TKey, TValueWrapper>(KeyEnumerator.Current, ValueEnumerator.Current);

            /// <inheritdoc/>
            public object Key => KeyEnumerator.Current;

            /// <inheritdoc/>
            public object Value => ValueEnumerator.Current;

            /// <inheritdoc/>
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

            /// <inheritdoc/>
            public void Dispose()
            {
                KeyEnumerator.Dispose();
                ValueEnumerator.Dispose();
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                bool keyEnumeratorResult = KeyEnumerator.MoveNext();
                bool valueEnumeratorResult = ValueEnumerator.MoveNext();

                return keyEnumeratorResult != valueEnumeratorResult ? throw new AccessViolationException() : keyEnumeratorResult;
            }

            /// <inheritdoc/>
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

            /// <inheritdoc/>
            public TValueWrapper Current { get; private set; }

            object IEnumerator.Current => Index == 0 || Index == SortedList.Count + 1 ? throw new InvalidOperationException() : Current;

            /// <inheritdoc/>
            public void Dispose()
            {
                Index = 0;
                Current = default;
            }

            /// <inheritdoc/>
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

            /// <inheritdoc/>
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
