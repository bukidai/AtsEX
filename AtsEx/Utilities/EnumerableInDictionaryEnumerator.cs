using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.Utilities
{
    internal class EnumerableInDictionaryEnumerator<TKey, TValue> : IEnumerator<TValue>
    {
        private readonly IDictionary<TKey, IEnumerable<TValue>> Source;
        private readonly IEnumerator<TKey> KeyEnumerator;

        private IEnumerator<TValue> CurrentEnumerator = null;

        public TValue Current => CurrentEnumerator.Current;
        object IEnumerator.Current => Current;

        public EnumerableInDictionaryEnumerator(IDictionary<TKey, IEnumerable<TValue>> source)
        {
            Source = source;
            KeyEnumerator = Source.Keys.GetEnumerator();

            Reset();
        }

        public void Dispose()
        {
            KeyEnumerator.Dispose();
            CurrentEnumerator?.Dispose();
        }

        public bool MoveNext()
        {
            if (KeyEnumerator.Current == null) return false;

            while (!CurrentEnumerator.MoveNext())
            {
                bool isKeyOver = !KeyEnumerator.MoveNext();
                if (isKeyOver) return false;

                CurrentEnumerator.Dispose();
                CurrentEnumerator = Source[KeyEnumerator.Current].GetEnumerator();
            }

            return true;
        }

        public void Reset()
        {
            KeyEnumerator.Reset();
            bool isEnd = !KeyEnumerator.MoveNext();
            if (isEnd) return;

            CurrentEnumerator?.Dispose();
            CurrentEnumerator = Source[KeyEnumerator.Current].GetEnumerator();
        }
    }
}
