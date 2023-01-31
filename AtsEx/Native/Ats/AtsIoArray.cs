using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.Native
{
    internal class AtsIoArray : IEnumerable<int>
    {
        private readonly IntPtr Address;

        public int Length { get; }

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

        public AtsIoArray(IntPtr source, int length = 256)
        {
            Address = source;
            Length = length;
        }

        public IEnumerator<int> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


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
