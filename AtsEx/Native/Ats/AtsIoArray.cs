using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.Native.Ats
{
    internal class AtsIoArray : IList<int>
    {
        private readonly IntPtr Address;

        public int Length { get; }

        int ICollection<int>.Count => Length;

        bool ICollection<int>.IsReadOnly { get; } = false;

        public unsafe int this[int index]
        {
            get
            {
                if ((index >= Length) || (index < 0))
                {
                    throw new IndexOutOfRangeException("Unmanaged array index is out of range: " + AppDomain.CurrentDomain.BaseDirectory);
                }

                int* pointer = (int*)Address.ToPointer();
                return pointer[index];
            }
            set
            {
                if ((index >= Length) || (index < 0))
                {
                    throw new IndexOutOfRangeException("Unmanaged array index is out of range: " + AppDomain.CurrentDomain.BaseDirectory);
                }

                int* pointer = (int*)Address.ToPointer();
                pointer[index] = value;
            }
        }

        public AtsIoArray(IntPtr source, int length = 1024)
        {
            Address = source;
            Length = length;
        }

        public IEnumerator<int> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(int item)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this[i] == item) return i;
            }

            return -1;
        }

        public bool Contains(int item) => IndexOf(item) != -1;

        void IList<int>.Insert(int index, int item) => throw new NotSupportedException();
        void IList<int>.RemoveAt(int index) => throw new NotSupportedException();
        void ICollection<int>.Add(int item) => throw new NotSupportedException();
        void ICollection<int>.Clear() => throw new NotSupportedException();
        void ICollection<int>.CopyTo(int[] array, int arrayIndex) => throw new NotSupportedException();
        bool ICollection<int>.Remove(int item) => throw new NotSupportedException();

        private class Enumerator : IEnumerator<int>
        {
            private readonly AtsIoArray Source;
            private int CurrentIndex = -1;

            public int Current => Source[CurrentIndex];
            object IEnumerator.Current => Current;

            public Enumerator(AtsIoArray source)
            {
                Source = source;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                CurrentIndex++;
                return CurrentIndex < Source.Length;
            }

            public void Reset() => CurrentIndex = -1;
        }
    }
}
